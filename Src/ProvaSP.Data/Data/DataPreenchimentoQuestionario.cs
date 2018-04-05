using Dapper;
using ProvaSP.Data.Funcionalidades;
using ProvaSP.Model.Entidades;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Net;
using System.Net.Http;
//using System.Web.
namespace ProvaSP.Data
{
    public static class DataPreenchimentoDeQuestionario
    {

        public static List<string> Sincronizar(List<QuestionarioUsuario> preenchimentosDeQuestionario, string ip, string userAgent)
        {
            string Edicao = Funcionalidades.Prova.Edicao;
            DateTime DataInicioAplicacao;
            var guidsSincronizados = new List<string>();
            using (var conn = new SqlConnection(StringsConexao.ProvaSP))
            {
                conn.Open();
                //Primeiro recupera a data do primeiro dia da aplicação
                DataInicioAplicacao = conn.ExecuteScalar<DateTime>(sql: "SELECT DataInicioAplicacao FROM ProvaEdicao WITH (NOLOCK) WHERE Edicao=@Edicao", param: new { Edicao = Edicao });
                using (SqlTransaction dbContextTransaction = conn.BeginTransaction())
                {
                    try
                    {

                        foreach (var preenchimentoDeQuestionario in preenchimentosDeQuestionario)
                        {
                            TipoQuestionario tipoQuestionario = (TipoQuestionario)preenchimentoDeQuestionario.QuestionarioID;
                            if (tipoQuestionario==TipoQuestionario.FichaRegistroAplicadorProva)
                            {
                                if (preenchimentoDeQuestionario.tur_id!=null)
                                {
                                    //Quando o item em questão for do tipo "FichaRegistroAplicadorProva", recupera a Escola com base na turma.
                                    preenchimentoDeQuestionario.esc_codigo = DataEscola.RecuperarCodigoEscolaComBaseNaTurma((int)preenchimentoDeQuestionario.tur_id, dbContextTransaction, conn);
                                }
                            }
                            else
                            {
                                //Do contrário a Escola é recuperada com base no vínculo do Usuário. O tipo de vínculo é inferido com base no tipo de Ficha/Questionário enviado.
                                //OBS: Supervisor não tem vínculo com uma Escola específica. Seu vícnculo é com a DRE.
                                int PerfilID = -1;
                                if (tipoQuestionario == TipoQuestionario.FichaRegistroDiretor || tipoQuestionario == TipoQuestionario.QuestionarioDiretor)
                                    PerfilID = (int)TipoPerfil.Diretor;
                                else if (tipoQuestionario == TipoQuestionario.FichaRegistroCoordenadorPedagogico || tipoQuestionario == TipoQuestionario.QuestionarioCoordenadorPedagogico)
                                    PerfilID = (int)TipoPerfil.CoordenadorPedagogico;
                                else if (tipoQuestionario == TipoQuestionario.QuestionarioProfessor)
                                    PerfilID = (int)TipoPerfil.Professor;
                                if (PerfilID>-1)
                                    preenchimentoDeQuestionario.esc_codigo = DataEscola.RecuperarCodigoEscolaComBaseNoPerfilDaPessoa(Edicao, preenchimentoDeQuestionario.usu_id, PerfilID, dbContextTransaction, conn);
                            }

                            if (string.IsNullOrEmpty(preenchimentoDeQuestionario.DataPreenchimento))
                            {
                                preenchimentoDeQuestionario.DataPreenchimento = DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'ss");
                            }

                            int QuestionarioUsuarioID = conn.ExecuteScalar<int>(
                                        sql: @"
                                    IF (@Guid='' OR NOT EXISTS(SELECT * FROM QuestionarioUsuario WITH (NOLOCK) WHERE Guid=@Guid))
                                    BEGIN
                                        INSERT INTO QuestionarioUsuario (QuestionarioID, Guid, esc_codigo, tur_id, usu_id, DataPreenchimento, IP, UserAgent, Edicao) VALUES (@QuestionarioID, @Guid, @esc_codigo, @tur_id, @usu_id, @DataPreenchimento, @IP, @UserAgent, @Edicao)
                                        SELECT @@IDENTITY
                                    END
                                    ELSE
                                        SELECT -1
                                    ",
                                        param: new
                                        {
                                            QuestionarioID = preenchimentoDeQuestionario.QuestionarioID,
                                            Guid = new DbString() { Value = preenchimentoDeQuestionario.Guid, IsAnsi = true, Length = 50 },
                                            esc_codigo = new DbString() { Value = preenchimentoDeQuestionario.esc_codigo, IsAnsi = true, Length = 20 },
                                            tur_id = preenchimentoDeQuestionario.tur_id,
                                            usu_id = new DbString() { Value = preenchimentoDeQuestionario.usu_id, IsAnsi = true, Length = 40 },
                                            DataPreenchimento = Convert.ToDateTime(preenchimentoDeQuestionario.DataPreenchimento),
                                            IP = new DbString() { Value = ip, IsAnsi = true, Length = 50 },
                                            UserAgent = new DbString() { Value = userAgent, IsAnsi = true, Length = 200 },
                                            Edicao = new DbString() { Value = Edicao, IsAnsi = true, Length = 10 }
                                        },
                                        transaction: dbContextTransaction);
                            if (QuestionarioUsuarioID > 0)
                            {

                                DataAcompanhamentoAplicacao.ProcessarInclusaoQuestionario(Edicao, DataInicioAplicacao, preenchimentoDeQuestionario, dbContextTransaction, conn);

                                foreach (var resposta in preenchimentoDeQuestionario.Respostas)
                                {
                                    System.Diagnostics.Debug.WriteLine(resposta.Numero);
                                    if (resposta.Valor=="default")
                                    {
                                        resposta.Valor = "";
                                    }
                                    conn.Execute(
                                    sql: @"
                                        DECLARE @QuestionarioItemID int
                                        SELECT @QuestionarioItemID=QuestionarioItemID FROM QuestionarioItem WITH (NOLOCK) WHERE QuestionarioID=@QuestionarioID AND Numero=@Numero
                                        INSERT INTO QuestionarioRespostaItem (QuestionarioUsuarioID, QuestionarioItemID, Valor) VALUES (@QuestionarioUsuarioID, @QuestionarioItemID, @Valor) 
                                        ",
                                    param: new
                                    {
                                        QuestionarioUsuarioID = QuestionarioUsuarioID,
                                        QuestionarioID = preenchimentoDeQuestionario.QuestionarioID,
                                        Numero = new DbString() { Value = resposta.Numero, IsAnsi = true, Length = 100 },
                                        Valor = new DbString() { Value = resposta.Valor, IsAnsi = true, Length = 500 }
                                    },
                                    transaction: dbContextTransaction);
                                }
                                guidsSincronizados.Add(preenchimentoDeQuestionario.Guid);
                            }
                        }
                        dbContextTransaction.Commit();
                        
                    }
                    catch (Exception ex)
                    {
                        guidsSincronizados.Clear();
                        dbContextTransaction.Rollback();
                        throw ex;
                    }
                }
            }
            return guidsSincronizados;
        }
    }
}
