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
    public static class DataTurma
    {
        public static List<Turma> RecuperarTurmasPorCalendario(int cal_id)
        {
            var retorno = new List<Turma>();
            using (var conn = new SqlConnection(StringsConexao.ProvaSP))
            {
                try
                {
                    retorno = conn.Query<Turma>(
                        sql: @"
                            SELECT tur_id, tur_codigo
                            FROM TUR_Turma
                            WHERE cal_id=@cal_id
                            ",
                        param: new
                        {
                            cal_id = cal_id
                        }
                        ).AsList<Turma>();
                    return retorno;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public static List<Turma> RecuperarCodigoTurmas(string ResultadoNivel, string Edicao, int AreaConhecimentoID, string AnoEscolar, string lista_esc_codigo)
        {
            var retorno = new List<Turma>();

            //var tabela = "ResultadoTurma";
            //if (ResultadoNivel == "ALUNO")
            //{
            //    tabela = "ResultadoAluno";
            //}

            var parametros = new DynamicParameters();

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

            try
            {
                if (Edicao == "ENTURMACAO_ATUAL")
                {
                    parametros.Add("cal_ano", DateTime.Now.Year, System.Data.DbType.Int32, System.Data.ParameterDirection.Input);

                    using (var conn = new SqlConnection(StringsConexao.GestaoEscolar))
                    {
                        retorno = conn.Query<Turma>(
                        sql: @"
                            SELECT t.tur_id, t.tur_codigo, e.esc_nome
                            FROM TUR_Turma t WITH (NOLOCK)
                            JOIN ESC_Escola e ON e.esc_id=t.esc_id
                            JOIN ACA_CalendarioAnual c ON c.cal_id=t.cal_id
                            WHERE t.tur_codigo NOT LIKE '%EJA%' AND LEFT(RIGHT(t.tur_codigo,2),1) = @AnoEscolar AND esc_codigo IN(" + sbEscolas.ToString() + @") AND c.cal_ano=@cal_ano
                            ORDER BY e.esc_nome, tur_codigo",
                        param: parametros
                        ).AsList<Turma>();
                        return retorno;
                    }
                }
                else
                {
                    using (var conn = new SqlConnection(StringsConexao.ProvaSP))
                    {
                        retorno = conn.Query<Turma>(
                        sql: @"
                            SELECT DISTINCT tur_codigo
                            FROM ResultadoAluno WITH (NOLOCK)
                            WHERE Edicao = @Edicao AND AreaConhecimentoID = @AreaConhecimentoID AND AnoEscolar = @AnoEscolar AND esc_codigo IN(" + sbEscolas.ToString() + @") 
                            ORDER BY tur_codigo",
                        param: parametros
                        ).AsList<Turma>();
                        return retorno;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            

        }


        public static List<ListaPresenca> RecuperarListaPresenca()
        {
            using (var conn = new SqlConnection(StringsConexao.ProvaSP))
            {
                return conn.Query<ListaPresenca>(
                    sql: @"
                            IF EXISTS (SELECT * FROM sysobjects WHERE name='Aluno' AND xtype='U')
                                SELECT * FROM Aluno
                                WHERE Edicao=(SELECT MAX(Edicao) FROM Aluno) 
                                ORDER BY tur_id, ChamadaAluno
                    "
                    ).AsList<ListaPresenca>();
            }
        }
    }
}
