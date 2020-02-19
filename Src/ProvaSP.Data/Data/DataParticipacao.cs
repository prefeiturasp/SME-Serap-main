using Dapper;
using ProvaSP.Data.Funcionalidades;
using ProvaSP.Model.Entidades;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace ProvaSP.Data
{
    public class DataParticipacao
    {
        public static List<Participacao> ParticipacaoSME(string Edicao, int AreaConhecimento, string AnoEscolar)
        {
            var resultado = new List<Participacao>();

            using (var conn = new SqlConnection(StringsConexao.ProvaSP))
            {
                conn.Open();

                resultado = conn.Query<Participacao>(
                                        sql: @"
                                            SELECT 'SME' AS Sigla, 'SME' AS Titulo, pGeral.AnoEscolar, 
                                                pGeral.TotalPrevisto AS TotalPrevistoGeral, pGeral.TotalPresente AS TotalPresenteGeral, pGeral.PercentualParticipacao AS PercentualParticipacaoGeral,
                                                pArea.AreaConhecimentoID, pArea.TotalPrevisto AS TotalPrevistoAreaConhecimento, 
                                                pArea.TotalPresente AS TotalPresenteAreaConhecimento, pArea.PercentualParticipacao AS PercentualParticipacaoAreaConhecimento
                                            FROM ParticipacaoSME pGeral WITH (NOLOCK)
                                                LEFT JOIN ParticipacaoSMEAreaConhecimento pArea WITH (NOLOCK) ON pArea.Edicao = pGeral.Edicao 
                                                    AND pArea.AnoEscolar = pGeral.AnoEscolar 
                                                    AND pArea.AreaConhecimentoID = @AreaConhecimentoID
                                            WHERE pGeral.Edicao = @Edicao
                                                AND pGeral.AnoEscolar = @AnoEscolar",
                                        param: new
                                        {
                                            Edicao = new DbString() { Value = Edicao, IsAnsi = true, Length = 10 },
                                            AreaConhecimentoID = AreaConhecimento,
                                            AnoEscolar = new DbString() { Value = AnoEscolar, IsAnsi = true, Length = 3 }
                                        }).ToList();
                resultado.ForEach(f =>
                {
                    f.Itens = ParticipacaoDRE(Edicao, AreaConhecimento, AnoEscolar, null, false);
                });
            }

            return resultado;
        }

        public static List<Participacao> ParticipacaoDRE(string Edicao, int AreaConhecimento, string AnoEscolar, string lista_uad_sigla, bool buscarItens)
        {
            var resultado = new List<Participacao>();
            List<string> lsDres = new List<string>();
            if (!string.IsNullOrEmpty(lista_uad_sigla))
            {
                lsDres = lista_uad_sigla.Split(',').ToList();
            }

            using (var conn = new SqlConnection(StringsConexao.ProvaSP))
            {
                conn.Open();

                string sql = @"SELECT pGeral.uad_sigla AS Sigla, pGeral.AnoEscolar,
                                    pGeral.TotalPrevisto AS TotalPrevistoGeral, pGeral.TotalPresente AS TotalPresenteGeral, pGeral.PercentualParticipacao AS PercentualParticipacaoGeral,
                                    pArea.AreaConhecimentoID, pArea.TotalPrevisto AS TotalPrevistoAreaConhecimento, 
                                    pArea.TotalPresente AS TotalPresenteAreaConhecimento, pArea.PercentualParticipacao AS PercentualParticipacaoAreaConhecimento
                               FROM ParticipacaoDRE pGeral WITH (NOLOCK)
                                LEFT JOIN ParticipacaoDREAreaConhecimento pArea WITH (NOLOCK) ON pArea.Edicao = pGeral.Edicao
                                    AND pArea.AnoEscolar = pGeral.AnoEscolar
                                    AND pArea.uad_sigla = pGeral.uad_sigla
                                    AND pArea.AreaConhecimentoID = @AreaConhecimentoID
                               WHERE pGeral.Edicao = @Edicao
                                    AND pGeral.AnoEscolar = @AnoEscolar";

                if (lsDres.Any())
                {
                    sql += " AND pGeral.uad_sigla IN @Dres";
                }

                sql += " ORDER BY pGeral.uad_sigla";

                resultado = conn.Query<Participacao>(
                                        sql: sql,
                                        param: new
                                        {
                                            Edicao = new DbString() { Value = Edicao, IsAnsi = true, Length = 10 },
                                            AreaConhecimentoID = AreaConhecimento,
                                            AnoEscolar = new DbString() { Value = AnoEscolar, IsAnsi = true, Length = 3 },
                                            Dres = lsDres
                                        }).ToList();

                foreach (var dre in resultado)
                {
                    dre.Titulo = DataDRE.RecuperarNome(dre.Sigla);
                    if (buscarItens)
                    {
                        dre.Itens = ParticipacaoEscola(Edicao, AreaConhecimento, AnoEscolar, dre.Sigla, null, false);
                    }
                }
            }

            return resultado;
        }

        public static List<Participacao> ParticipacaoEscola(string Edicao, int AreaConhecimento, string AnoEscolar, string uad_sigla, string lista_esc_codigo, bool buscarItens)
        {
            var resultado = new List<Participacao>();
            List<string> lsEscola = new List<string>();
            if (!string.IsNullOrEmpty(lista_esc_codigo))
            {
                lsEscola = lista_esc_codigo.Split(',').ToList();
            }

            using (var conn = new SqlConnection(StringsConexao.ProvaSP))
            {
                conn.Open();

                string sql = @"SELECT pGeral.esc_codigo AS Sigla, e.esc_nome AS Titulo, pGeral.AnoEscolar,
                                    pGeral.TotalPrevisto AS TotalPrevistoGeral, pGeral.TotalPresente AS TotalPresenteGeral, pGeral.PercentualParticipacao AS PercentualParticipacaoGeral,
                                    pArea.AreaConhecimentoID, pArea.TotalPrevisto AS TotalPrevistoAreaConhecimento, 
                                    pArea.TotalPresente AS TotalPresenteAreaConhecimento, pArea.PercentualParticipacao AS PercentualParticipacaoAreaConhecimento
                               FROM ParticipacaoEscola pGeral WITH (NOLOCK)
                                    LEFT JOIN Escola (NOLOCK) e ON e.esc_codigo = pGeral.esc_codigo 
                                    LEFT JOIN ParticipacaoEscolaAreaConhecimento pArea WITH (NOLOCK) ON pArea.Edicao = pGeral.Edicao
                                        AND pArea.AnoEscolar = pGeral.AnoEscolar
                                        AND pArea.uad_sigla = pGeral.uad_sigla
                                        AND pArea.esc_codigo = pGeral.esc_codigo
                                        AND pArea.AreaConhecimentoID = @AreaConhecimentoID
                               WHERE pGeral.Edicao = @Edicao AND pGeral.AnoEscolar = @AnoEscolar ";

                if (!string.IsNullOrEmpty(uad_sigla))
                {
                    sql += "AND pGeral.uad_sigla = @uad_sigla ";
                }

                if (lsEscola.Any())
                {
                    sql += "AND pGeral.esc_codigo IN @Escolas ";
                }

                sql += "ORDER BY pGeral.esc_codigo";

                resultado = conn.Query<Participacao>(
                                        sql: sql,
                                        param: new
                                        {
                                            Edicao = new DbString() { Value = Edicao, IsAnsi = true, Length = 10 },
                                            AreaConhecimentoID = AreaConhecimento,
                                            AnoEscolar = new DbString() { Value = AnoEscolar, IsAnsi = true, Length = 3 },
                                            uad_sigla = new DbString() { Value = uad_sigla, IsAnsi = true, Length = 3 },
                                            Escolas = lsEscola
                                        }).ToList();

                foreach (var esc in resultado)
                {
                    if (buscarItens)
                    {
                        esc.Itens = ParticipacaoTurma(Edicao, AreaConhecimento, AnoEscolar, esc.Sigla, null, false);
                    }
                }
            }

            return resultado;
        }

        public static List<Participacao> ParticipacaoTurma(string Edicao, int AreaConhecimento, string AnoEscolar, string lista_esc_codigo, string lista_turmas, bool apresentarEscola)
        {
            var resultado = new List<Participacao>();

            List<string> lsEscola = new List<string>();
            if (!string.IsNullOrEmpty(lista_esc_codigo))
            {
                lsEscola = lista_esc_codigo.Split(',').ToList();
            }

            List<string> lsTurma = new List<string>();
            if (!string.IsNullOrEmpty(lista_turmas))
            {
                lsTurma = lista_turmas.Split(',').ToList();
            }

            using (var conn = new SqlConnection(StringsConexao.ProvaSP))
            {
                conn.Open();

                string sql = @"SELECT pGeral.tur_codigo + ISNULL(' (' + e.esc_codigo + ')', '') AS Sigla, 
                                    pGeral.tur_codigo" + (apresentarEscola ? " + ISNULL(' - ' + e.esc_nome, '')" : string.Empty) + @" AS Titulo,
                                    pGeral.AnoEscolar,
                                    pGeral.TotalPrevisto AS TotalPrevistoGeral, pGeral.TotalPresente AS TotalPresenteGeral, pGeral.PercentualParticipacao AS PercentualParticipacaoGeral,
                                    pArea.AreaConhecimentoID, pArea.TotalPrevisto AS TotalPrevistoAreaConhecimento, 
                                    pArea.TotalPresente AS TotalPresenteAreaConhecimento, pArea.PercentualParticipacao AS PercentualParticipacaoAreaConhecimento
                               FROM ParticipacaoTurma pGeral WITH (NOLOCK)
                                    LEFT JOIN Escola (NOLOCK) e ON e.esc_codigo = pGeral.esc_codigo 
                                    LEFT JOIN ParticipacaoTurmaAreaConhecimento pArea WITH (NOLOCK) ON pArea.Edicao = pGeral.Edicao
                                        AND pArea.AnoEscolar = pGeral.AnoEscolar
                                        AND pArea.uad_sigla = pGeral.uad_sigla
                                        AND pArea.esc_codigo = pGeral.esc_codigo
                                        AND pArea.tur_codigo = pGeral.tur_codigo
                                        AND pArea.AreaConhecimentoID = @AreaConhecimentoID
                               WHERE pGeral.Edicao = @Edicao AND pGeral.AnoEscolar = @AnoEscolar ";

                if (lsEscola.Any())
                {
                    sql += "AND pGeral.esc_codigo IN @Escolas ";
                }

                if (lsTurma.Any())
                {
                    sql += "AND pGeral.tur_codigo IN @Turmas ";
                }

                sql += "ORDER BY pGeral.esc_codigo, pGeral.tur_codigo";

                resultado = conn.Query<Participacao>(
                                        sql: sql,
                                        param: new
                                        {
                                            Edicao = new DbString() { Value = Edicao, IsAnsi = true, Length = 10 },
                                            AreaConhecimentoID = AreaConhecimento,
                                            AnoEscolar = new DbString() { Value = AnoEscolar, IsAnsi = true, Length = 3 },
                                            Escolas = lsEscola,
                                            Turmas = lsTurma
                                        }).ToList();
            }

            return resultado;
        }
    }
}
