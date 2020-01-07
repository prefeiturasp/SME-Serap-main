using Dapper;
using ProvaSP.Data.Funcionalidades;
using ProvaSP.Model.Entidades;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace ProvaSP.Data.Data
{
    public static class DataFatorAssociado
    {
        public static List<Questionario> RetornarQuestionario(string edicao)
        {
            using (var conn = new SqlConnection(StringsConexao.ProvaSP))
            {
                return conn.Query<Questionario>(
                    sql: @"SELECT f.FatorAssociadoQuestionarioId AS QuestionarioID, f.Edicao, f.Nome 
                           FROM FatorAssociadoQuestionario f WITH(NOLOCK)
                           WHERE Edicao = @Edicao AND f.FatorAssociadoQuestionarioId NOT IN (4,3,7,8) 
                           ORDER BY f.Nome",
                            param: new
                            {
                                Edicao = new DbString() { Value = edicao, IsAnsi = true, Length = 10 }
                            }
                    ).ToList();
            }
        }

        public static List<Constructo> RetornarConstructo(string edicao, int cicloId, int questionarioId)
        {
            using (var conn = new SqlConnection(StringsConexao.ProvaSP))
            {
                return conn.Query<Constructo>(
                    sql: @"SELECT c.ConstructoId, c.Edicao, c.CicloId, c.FatorAssociadoQuestionarioId AS QuestionarioId, c.Nome 
                           FROM Constructo c WITH(NOLOCK)
                           WHERE c.Edicao = @Edicao AND c.CicloId = @CicloId AND c.FatorAssociadoQuestionarioId = @QuestionarioId
                           ORDER BY c.Nome",
                            param: new
                            {
                                Edicao = new DbString() { Value = edicao, IsAnsi = true, Length = 10 },
                                CicloId = new DbString() { Value = cicloId.ToString(), IsAnsi = true, Length = 10 },
                                QuestionarioId = new DbString() { Value = questionarioId.ToString(), IsAnsi = true, Length = 10 }
                            }
                    ).ToList();
            }
        }

        public static FatorAssociado RetornarFatorAssociado(string edicao, int cicloId, int questionarioId, int constructoId)
        {
            using (var conn = new SqlConnection(StringsConexao.ProvaSP))
            {
                FatorAssociado fatorAssociado = new FatorAssociado();

                fatorAssociado.Constructo = conn.Query<Constructo>(
                    sql: @"SELECT c.ConstructoId, c.Edicao, c.CicloId, c.FatorAssociadoQuestionarioId AS QuestionarioId, c.Nome 
                           FROM Constructo c WITH(NOLOCK)
                           WHERE c.ConstructoId = @ConstructoId",
                            param: new
                            {
                                ConstructoId = new DbString() { Value = constructoId.ToString(), IsAnsi = true, Length = 10 }
                            }
                    ).FirstOrDefault();

                fatorAssociado.Resultados = conn.Query<FatorAssociadoResultado>(
                    sql: @"SELECT f.AreaConhecimentoId, a.Nome AS AreaConhecimentoNome, f.Pontos
                           FROM FatorAssociado f WITH(NOLOCK)
                                INNER JOIN AreaConhecimento a ON a.AreaConhecimentoID = f.AreaConhecimentoId
                           WHERE f.ConstructoId = @ConstructoId 
                           ORDER BY a.Nome",
                            param: new
                            {
                                ConstructoId = new DbString() { Value = constructoId.ToString(), IsAnsi = true, Length = 10 }
                            }
                    ).ToList();

                fatorAssociado.Variaveis = conn.Query<Variavel>(
                    sql: @"SELECT f.VariavelId, f.VariavelDescricao 
                           FROM FatorAssociadoQuestionarioRespostaSME f WITH(NOLOCK)
                           WHERE f.Edicao = @Edicao AND f.CicloId = @CicloId AND f.FatorAssociadoQuestionarioId = @QuestionarioId AND f.ConstructoId = @ConstructoId
                           GROUP BY f.VariavelId, f.VariavelDescricao
                           ORDER BY CAST(f.VariavelId AS NUMERIC)",
                            param: new
                            {
                                Edicao = new DbString() { Value = edicao, IsAnsi = true, Length = 10 },
                                CicloId = new DbString() { Value = cicloId.ToString(), IsAnsi = true, Length = 10 },
                                QuestionarioId = new DbString() { Value = questionarioId.ToString(), IsAnsi = true, Length = 10 },
                                ConstructoId = new DbString() { Value = constructoId.ToString(), IsAnsi = true, Length = 10 }
                            }
                    ).ToList();

                return fatorAssociado;
            }
        }

        public static List<VariavelItem> RetornarResultadoItemSME(string edicao, int cicloId, int questionarioId, int constructoId)
        {
            using (var conn = new SqlConnection(StringsConexao.ProvaSP))
            {
                string sql = @"SELECT f.VariavelDescricao, f.ItemDescricao, f.Valor
                           FROM FatorAssociadoQuestionarioRespostaSME f
                           WHERE f.Edicao = @Edicao AND f.CicloId = @CicloId AND f.FatorAssociadoQuestionarioId = @QuestionarioId ";

                if (constructoId > 0)
                {
                    sql += "AND f.ConstructoId = @ConstructoId ";
                }

                sql += "ORDER BY CAST(f.VariavelId AS NUMERIC), f.ItemDescricao";

                return conn.Query<VariavelItem>(
                    sql: sql,
                            param: new
                            {
                                Edicao = new DbString() { Value = edicao, IsAnsi = true, Length = 10 },
                                CicloId = new DbString() { Value = cicloId.ToString(), IsAnsi = true, Length = 10 },
                                QuestionarioId = new DbString() { Value = questionarioId.ToString(), IsAnsi = true, Length = 10 },
                                ConstructoId = new DbString() { Value = constructoId.ToString(), IsAnsi = true, Length = 10 }
                            }
                    ).ToList();
            }
        }

        public static List<VariavelItem> RetornarResultadoItemDRE(string edicao, int cicloId, int questionarioId, string uad_sigla)
        {
            using (var conn = new SqlConnection(StringsConexao.ProvaSP))
            {
                return conn.Query<VariavelItem>(
                    sql: @"SELECT f.VariavelDescricao, f.ItemDescricao, f.Valor, fSup.Valor AS ValorSuperior
                           FROM FatorAssociadoQuestionarioRespostaDRE f
	                            INNER JOIN FatorAssociadoQuestionarioRespostaSME fSup
		                            ON fSup.Edicao = f.Edicao
		                            AND fSup.CicloId = f.CicloId
		                            AND fSup.FatorAssociadoQuestionarioId = f.FatorAssociadoQuestionarioId
		                            AND fSup.VariavelId = f.VariavelId
		                            AND fSup.ItemId = f.ItemId
                           WHERE f.Edicao = @Edicao AND f.CicloId = @CicloId AND f.FatorAssociadoQuestionarioId = @QuestionarioId AND f.uad_sigla = @uad_sigla
                           ORDER BY CAST(f.VariavelId AS NUMERIC), f.ItemDescricao",
                            param: new
                            {
                                Edicao = new DbString() { Value = edicao, IsAnsi = true, Length = 10 },
                                CicloId = new DbString() { Value = cicloId.ToString(), IsAnsi = true, Length = 10 },
                                QuestionarioId = new DbString() { Value = questionarioId.ToString(), IsAnsi = true, Length = 10 },
                                uad_sigla = new DbString() { Value = uad_sigla, IsAnsi = true, Length = 10 }
                            }
                    ).ToList();
            }
        }

        public static List<VariavelItem> RetornarResultadoItemEscola(string edicao, int cicloId, int questionarioId, string uad_sigla, string esc_codigo)
        {
            using (var conn = new SqlConnection(StringsConexao.ProvaSP))
            {
                return conn.Query<VariavelItem>(
                    sql: @"SELECT f.VariavelDescricao, f.ItemDescricao, f.Valor, fSup.Valor AS ValorSuperior
                           FROM FatorAssociadoQuestionarioRespostaEscola f
	                            INNER JOIN FatorAssociadoQuestionarioRespostaDRE fSup
		                            ON fSup.Edicao = f.Edicao
		                            AND fSup.CicloId = f.CicloId
		                            AND fSup.FatorAssociadoQuestionarioId = f.FatorAssociadoQuestionarioId
		                            AND fSup.VariavelId = f.VariavelId
		                            AND fSup.ItemId = f.ItemId
                                    AND fSup.uad_sigla = f.uad_sigla
                           WHERE f.Edicao = @Edicao AND f.CicloId = @CicloId AND f.FatorAssociadoQuestionarioId = @QuestionarioId AND f.uad_sigla = @uad_sigla AND f.esc_codigo = @esc_codigo
                           ORDER BY CAST(f.VariavelId AS NUMERIC), f.ItemDescricao",
                            param: new
                            {
                                Edicao = new DbString() { Value = edicao, IsAnsi = true, Length = 10 },
                                CicloId = new DbString() { Value = cicloId.ToString(), IsAnsi = true, Length = 10 },
                                QuestionarioId = new DbString() { Value = questionarioId.ToString(), IsAnsi = true, Length = 10 },
                                uad_sigla = new DbString() { Value = uad_sigla, IsAnsi = true, Length = 10 },
                                esc_codigo = new DbString() { Value = esc_codigo, IsAnsi = true, Length = 10 }
                            }
                    ).ToList();
            }
        }
    }
}
