using Dapper;
using ProvaSP.Data.Funcionalidades;
using ProvaSP.Model.Entidades;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProvaSP.Data
{
    public static class DataAluno
    {

        public static string[] RecuperarParticipacoesEmEdicoes(string alu_matricula)
        {
            string[] retorno = null;
            using (var conn = new SqlConnection(StringsConexao.ProvaSP))
            {
                retorno = conn.Query<string>(sql: "SELECT DISTINCT Edicao FROM ResultadoAluno WHERE alu_matricula=@alu_matricula ORDER BY Edicao DESC", param: new { alu_matricula = new DbString() { Value = alu_matricula, IsAnsi = true, Length = 50 } }).ToArray<string>();
            }
            return retorno;
        }

        public static List<Aluno> RecuperarAlunos(string Edicao, int AreaConhecimentoID, string AnoEscolar, string lista_esc_codigo, string lista_turmas)
        {
            var retorno = new List<Aluno>();

            var parametros = new DynamicParameters();

            if (Edicao == "ENTURMACAO_ATUAL")
            {
                using (var conn = new SqlConnection(StringsConexao.GestaoEscolar))
                {
                    var sbTurmas = new StringBuilder();
                    var lista_turmasSplit = lista_turmas.Split(',');
                    
                    //Construção dos parâmetros passados pela variável lista_turmas de modo a evitar sql injection.
                    foreach (var tur_id in lista_turmasSplit)
                    {
                        if (sbTurmas.Length > 0)
                        {
                            sbTurmas.Append(",");
                        }
                        string parameterName = "@p_" + tur_id;
                        sbTurmas.Append(parameterName);
                        parametros.Add(parameterName, tur_id, System.Data.DbType.Int32, System.Data.ParameterDirection.Input);
                    }

                    try
                    {
                        retorno = conn.Query<Aluno>(
                            sql: @"
                            SELECT a.alu_matricula, a.alu_nome as Nome, t.tur_codigo, e.esc_nome, e.esc_codigo
                            FROM ACA_Aluno a WITH (NOLOCK)
                            JOIN MTR_MatriculaTurma m ON a.alu_id = m.alu_id
                            JOIN TUR_Turma t ON t.tur_id=m.tur_id
                            JOIN ESC_Escola e ON e.esc_id=t.esc_id
                            WHERE t.tur_id IN(" + sbTurmas.ToString() + @") 
                            ORDER BY t.tur_codigo, alu_nome",
                            param: parametros
                            ).AsList<Aluno>();
                        return retorno;
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
            else
            {
                using (var conn = new SqlConnection(StringsConexao.ProvaSP))
                {

                    parametros.Add("Edicao", Edicao, System.Data.DbType.AnsiString, System.Data.ParameterDirection.Input, 10);
                    parametros.Add("AnoEscolar", AnoEscolar, System.Data.DbType.AnsiString, System.Data.ParameterDirection.Input, 3);
                    parametros.Add("AreaConhecimentoID", AreaConhecimentoID, System.Data.DbType.Int32, System.Data.ParameterDirection.Input);

                    var sbEscolas = new StringBuilder();
                    var lista_esc_codigoSplit = lista_esc_codigo.Split(',');
                    if (lista_esc_codigoSplit.Length == 0)
                    {
                        return retorno;
                    }

                    //Construção dos parâmetros passados pela variável lista_esc_codigo de modo a evitar sql injection.
                    foreach (var esc_codigo in lista_esc_codigoSplit)
                    {
                        if (sbEscolas.Length > 0)
                        {
                            sbEscolas.Append(",");
                        }
                        string parameterName = "@p_" + esc_codigo;
                        sbEscolas.Append(parameterName);
                        parametros.Add(parameterName, esc_codigo, System.Data.DbType.AnsiString, System.Data.ParameterDirection.Input, 20);
                    }
                    
                    var sbTurmas = new StringBuilder();
                    var lista_turmasSplit = lista_turmas.Split(',');
                    if (lista_turmasSplit.Length == 0)
                    {
                        return retorno;
                    }

                    //Construção dos parâmetros passados pela variável lista_turmas de modo a evitar sql injection.
                    foreach (var turma in lista_turmasSplit)
                    {
                        if (sbTurmas.Length > 0)
                        {
                            sbTurmas.Append(",");
                        }
                        string parameterName = "@p_" + turma;
                        sbTurmas.Append(parameterName);
                        parametros.Add(parameterName, turma, System.Data.DbType.AnsiString, System.Data.ParameterDirection.Input, 20);
                    }

                    try
                    {
                        retorno = conn.Query<Aluno>(
                            sql: @"
                            SELECT alu_matricula, alu_nome as Nome
                            FROM ResultadoAluno WITH (NOLOCK)
                            WHERE Edicao = @Edicao AND AreaConhecimentoID = @AreaConhecimentoID AND AnoEscolar = @AnoEscolar AND Valor IS NOT NULL AND esc_codigo IN(" + sbEscolas.ToString() + ") AND tur_codigo IN(" + sbTurmas.ToString() + @") 
                            ORDER BY alu_nome",
                            param: parametros
                            ).AsList<Aluno>();
                        return retorno;
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }

            
            
        }

    }
}

