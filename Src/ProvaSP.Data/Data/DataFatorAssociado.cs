using Dapper;
using ProvaSP.Data.Funcionalidades;
using ProvaSP.Model.Entidades;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.InteropServices;

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
                           WHERE Edicao = @Edicao
                           ORDER BY f.Nome",
                            param: new
                            {
                                Edicao = new DbString() { Value = edicao, IsAnsi = true, Length = 10 }
                            }
                    ).ToList();
            }
        }

        public static List<Constructo> RetornarConstructo(string edicao, int? cicloId, int? anoEscolar, int questionarioId)
        {
            using (var conn = new SqlConnection(StringsConexao.ProvaSP))
            {
                var query = @"SELECT c.ConstructoId, c.Edicao, c.CicloId, c.FatorAssociadoQuestionarioId AS QuestionarioId, c.Nome
                           FROM Constructo c WITH(NOLOCK)
                           WHERE c.Edicao = @Edicao AND c.FatorAssociadoQuestionarioId = @QuestionarioId ";

                if (cicloId != null) query += " AND c.CicloId = @CicloId ";
                if (anoEscolar != null) query += " AND c.AnoEscolar = @AnoEscolar ";

                query += " ORDER BY c.Nome";

                return conn.Query<Constructo>(query,
                    param: new
                    {
                        Edicao = new DbString() { Value = edicao, IsAnsi = true, Length = 10 },
                        CicloId = new DbString() { Value = cicloId?.ToString(), IsAnsi = true, Length = 10 },
                        AnoEscolar = new DbString() { Value = anoEscolar?.ToString(), IsAnsi = true, Length = 1 },
                        QuestionarioId = new DbString() { Value = questionarioId.ToString(), IsAnsi = true, Length = 10 }
                    }).ToList();
            }
        }

        public static FatorAssociado RetornarFatorAssociado(string edicao, int? cicloId, int questionarioId, int constructoId)
        {
            using (var conn = new SqlConnection(StringsConexao.ProvaSP))
            {
                FatorAssociado fatorAssociado = new FatorAssociado();

                fatorAssociado.Constructo = conn.Query<Constructo>(
                    sql: @"SELECT c.ConstructoId, c.Edicao, c.CicloId, c.AnoEscolar, c.FatorAssociadoQuestionarioId AS QuestionarioId, c.Nome, c.Referencia
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

                var queryFatorAssociado = @"SELECT f.VariavelId, f.VariavelDescricao
                                          FROM FatorAssociadoQuestionarioRespostaSME f WITH(NOLOCK)
                                          INNER JOIN FatorAssociadoQuestionarioRespostaSMEConstructo fc (NOLOCK)
                                            ON f.FatorAssociadoQuestionarioRespostaSMEId = fc.FatorAssociadoQuestionarioRespostaSMEId
                                          WHERE f.Edicao = @Edicao AND f.FatorAssociadoQuestionarioId = @QuestionarioId AND fc.ConstructoId = @ConstructoId ";

                if (cicloId != null)  queryFatorAssociado += " AND f.CicloId = @CicloId ";

                queryFatorAssociado += @" GROUP BY f.VariavelId, f.VariavelDescricao
                                       ORDER BY CAST(f.VariavelId AS NUMERIC)";

                fatorAssociado.Variaveis = conn.Query<Variavel>(queryFatorAssociado,
                            param: new
                            {
                                Edicao = new DbString() { Value = edicao, IsAnsi = true, Length = 10 },
                                CicloId = new DbString() { Value = cicloId?.ToString(), IsAnsi = true, Length = 10 },
                                QuestionarioId = new DbString() { Value = questionarioId.ToString(), IsAnsi = true, Length = 10 },
                                ConstructoId = new DbString() { Value = constructoId.ToString(), IsAnsi = true, Length = 10 }
                            }
                    ).ToList();

                return fatorAssociado;
            }
        }

        public static List<VariavelItem> RetornarResultadoItemSME(string edicao, int? cicloId, int? anoEscolar, int questionarioId, int constructoId)
        {
            using (var conn = new SqlConnection(StringsConexao.ProvaSP))
            {
                var query = @"SELECT f.VariavelDescricao, f.ItemDescricao, f.Valor
                           FROM FatorAssociadoQuestionarioRespostaSME f ";

                if (constructoId > 0)
                    query += @"INNER JOIN FatorAssociadoQuestionarioRespostaSMEConstructo fc (NOLOCK)
                            ON f.FatorAssociadoQuestionarioRespostaSMEId = fc.FatorAssociadoQuestionarioRespostaSMEId ";

                query += " WHERE f.Edicao = @Edicao AND f.FatorAssociadoQuestionarioId = @QuestionarioId ";

                if(cicloId != null) query += " AND f.CicloId = @CicloId ";
                if(anoEscolar != null) query += " AND f.AnoEscolar = @AnoEscolar ";
                if (constructoId > 0) query += "AND fc.ConstructoId = @ConstructoId ";

                query += "ORDER BY CAST(f.VariavelId AS NUMERIC), f.ItemDescricao";

                return conn.Query<VariavelItem>(
                    sql: query,
                            param: new
                            {
                                Edicao = new DbString() { Value = edicao, IsAnsi = true, Length = 10 },
                                AnoEscolar = new DbString() { Value = anoEscolar?.ToString(), IsAnsi = true, Length = 4 },
                                CicloId = new DbString() { Value = cicloId?.ToString(), IsAnsi = true, Length = 10 },
                                QuestionarioId = new DbString() { Value = questionarioId.ToString(), IsAnsi = true, Length = 10 },
                                ConstructoId = new DbString() { Value = constructoId.ToString(), IsAnsi = true, Length = 10 }
                            }
                    ).ToList();
            }
        }

        public static List<VariavelItem> RetornarResultadoItemDRE(string edicao, int? cicloId, int? anoEscolar, int questionarioId, string uad_sigla)
        {
            var query = @"SELECT f.VariavelDescricao, f.ItemDescricao, f.Valor, fSup.Valor AS ValorSuperior
                        FROM FatorAssociadoQuestionarioRespostaDRE f
	                    INNER JOIN FatorAssociadoQuestionarioRespostaSME fSup
		                    ON fSup.Edicao = f.Edicao
		                    AND fSup.CicloId = f.CicloId
                            AND fSup.AnoEscolar = f.AnoEscolar
		                    AND fSup.FatorAssociadoQuestionarioId = f.FatorAssociadoQuestionarioId
		                    AND fSup.VariavelId = f.VariavelId
		                    AND fSup.ItemId = f.ItemId
                        WHERE f.Edicao = @Edicao AND f.FatorAssociadoQuestionarioId = @QuestionarioId AND f.uad_sigla = @uad_sigla ";

            if (cicloId != null) query += " AND f.CicloId = @CicloId ";

            if (anoEscolar != null) query += " AND f.AnoEscolar = @AnoEscolar ";

            query += " ORDER BY CAST(f.VariavelId AS NUMERIC), f.ItemDescricao";

            using (var conn = new SqlConnection(StringsConexao.ProvaSP))
            {
                return conn.Query<VariavelItem>(query,
                    param: new
                    {
                        Edicao = new DbString() { Value = edicao, IsAnsi = true, Length = 10 },
                        AnoEscolar = new DbString() { Value = anoEscolar?.ToString(), IsAnsi = true, Length = 4 },
                        CicloId = new DbString() { Value = cicloId.ToString(), IsAnsi = true, Length = 10 },
                        QuestionarioId = new DbString() { Value = questionarioId.ToString(), IsAnsi = true, Length = 10 },
                        uad_sigla = new DbString() { Value = uad_sigla, IsAnsi = true, Length = 10 }
                    }).ToList();
            }
        }

        public static List<VariavelItem> RetornarResultadoItemEscola(string edicao, int? cicloId, int? anoEscolar, int questionarioId, string uad_sigla, 
            string esc_codigo)
        {
            var query = @"SELECT f.VariavelDescricao, f.ItemDescricao, f.Valor, fSup.Valor AS ValorSuperior
                        FROM FatorAssociadoQuestionarioRespostaEscola f
	                    INNER JOIN FatorAssociadoQuestionarioRespostaDRE fSup
		                    ON fSup.Edicao = f.Edicao
		                    AND fSup.CicloId = f.CicloId
                            AND fSup.AnoEscolar = f.AnoEscolar
		                    AND fSup.FatorAssociadoQuestionarioId = f.FatorAssociadoQuestionarioId
		                    AND fSup.VariavelId = f.VariavelId
		                    AND fSup.ItemId = f.ItemId
                            AND fSup.uad_sigla = f.uad_sigla
                        WHERE f.Edicao = @Edicao AND f.FatorAssociadoQuestionarioId = @QuestionarioId AND f.uad_sigla = @uad_sigla AND f.esc_codigo = @esc_codigo ";

            if (cicloId != null) query += " AND f.CicloId = @CicloId ";

            if (anoEscolar != null) query += " AND f.AnoEscolar = @AnoEscolar ";

            query += " ORDER BY CAST(f.VariavelId AS NUMERIC), f.ItemDescricao";

            using (var conn = new SqlConnection(StringsConexao.ProvaSP))
            {
                return conn.Query<VariavelItem>(query,
                    param: new
                    {
                        Edicao = new DbString() { Value = edicao, IsAnsi = true, Length = 10 },
                        AnoEscolar = new DbString() { Value = anoEscolar?.ToString(), IsAnsi = true, Length = 4 },
                        CicloId = new DbString() { Value = cicloId.ToString(), IsAnsi = true, Length = 10 },
                        QuestionarioId = new DbString() { Value = questionarioId.ToString(), IsAnsi = true, Length = 10 },
                        uad_sigla = new DbString() { Value = uad_sigla, IsAnsi = true, Length = 10 },
                        esc_codigo = new DbString() { Value = esc_codigo, IsAnsi = true, Length = 10 }
                    }).ToList();
            }
        }
    }
}