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
        public static List<Participacao> ParticipacaoSME(string Edicao, string AnoEscolar)
        {
            var resultado = new List<Participacao>();

            using (var conn = new SqlConnection(StringsConexao.ProvaSP))
            {
                conn.Open();

                resultado = conn.Query<Participacao>(
                                        sql: @"
                                            SELECT 'SME' AS Sigla, 'SME' AS Titulo, AnoEscolar, TotalPrevisto, TotalPresente, PercentualParticipacao
                                            FROM ParticipacaoSME WITH (NOLOCK)
                                            WHERE Edicao = @Edicao AND AnoEscolar = @AnoEscolar",
                                        param: new
                                        {
                                            Edicao = new DbString() { Value = Edicao, IsAnsi = true, Length = 10 },
                                            AnoEscolar = new DbString() { Value = AnoEscolar, IsAnsi = true, Length = 3 }
                                        }).ToList();
                resultado.ForEach(f =>
                {
                    f.Itens = ParticipacaoDRE(Edicao, AnoEscolar, null, false);
                });
            }

            return resultado;
        }

        public static List<Participacao> ParticipacaoDRE(string Edicao, string AnoEscolar, string lista_uad_sigla, bool buscarItens)
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

                string sql = @"SELECT uad_sigla AS Sigla, AnoEscolar, TotalPrevisto, TotalPresente, PercentualParticipacao
                               FROM ParticipacaoDRE p WITH (NOLOCK)
                               WHERE Edicao = @Edicao AND AnoEscolar = @AnoEscolar ";

                if (lsDres.Any())
                {
                    sql += "AND uad_sigla IN @Dres ";
                }

                sql += "ORDER BY p.uad_sigla";

                resultado = conn.Query<Participacao>(
                                        sql: sql,
                                        param: new
                                        {
                                            Edicao = new DbString() { Value = Edicao, IsAnsi = true, Length = 10 },
                                            AnoEscolar = new DbString() { Value = AnoEscolar, IsAnsi = true, Length = 3 },
                                            Dres = lsDres
                                        }).ToList();

                foreach (var dre in resultado)
                {
                    dre.Titulo = DataDRE.RecuperarNome(dre.Sigla);
                    if (buscarItens)
                    {
                        dre.Itens = ParticipacaoEscola(Edicao, AnoEscolar, dre.Sigla, null, false);
                    }
                }
            }

            return resultado;
        }

        public static List<Participacao> ParticipacaoEscola(string Edicao, string AnoEscolar, string uad_sigla, string lista_esc_codigo, bool buscarItens)
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

                string sql = @"SELECT p.esc_codigo AS Sigla, e.esc_nome AS Titulo, p.AnoEscolar, p.TotalPrevisto, p.TotalPresente, p.PercentualParticipacao
                               FROM ParticipacaoEscola p WITH (NOLOCK)
                                    LEFT JOIN Escola (NOLOCK) e ON e.esc_codigo = p.esc_codigo 
                               WHERE Edicao = @Edicao AND AnoEscolar = @AnoEscolar ";

                if (!string.IsNullOrEmpty(uad_sigla))
                {
                    sql += "AND p.uad_sigla = @uad_sigla ";
                }

                if (lsEscola.Any())
                {
                    sql += "AND p.esc_codigo IN @Escolas ";
                }

                sql += "ORDER BY p.esc_codigo";

                resultado = conn.Query<Participacao>(
                                        sql: sql,
                                        param: new
                                        {
                                            Edicao = new DbString() { Value = Edicao, IsAnsi = true, Length = 10 },
                                            AnoEscolar = new DbString() { Value = AnoEscolar, IsAnsi = true, Length = 3 },
                                            uad_sigla = new DbString() { Value = uad_sigla, IsAnsi = true, Length = 3 },
                                            Escolas = lsEscola
                                        }).ToList();

                foreach (var esc in resultado)
                {
                    if (buscarItens)
                    {
                        esc.Itens = ParticipacaoTurma(Edicao, AnoEscolar, esc.Sigla, null);
                    }
                }
            }

            return resultado;
        }

        public static List<Participacao> ParticipacaoTurma(string Edicao, string AnoEscolar, string lista_esc_codigo, string lista_turmas)
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

                string sql = @"SELECT p.tur_codigo + ISNULL(' (' + e.esc_codigo + ')', '') AS Sigla, p.tur_codigo AS Titulo, p.AnoEscolar, p.TotalPrevisto, p.TotalPresente, p.PercentualParticipacao
                               FROM ParticipacaoTurma p WITH (NOLOCK)
                                    LEFT JOIN Escola (NOLOCK) e ON e.esc_codigo = p.esc_codigo 
                               WHERE Edicao = @Edicao AND AnoEscolar = @AnoEscolar ";

                if (lsEscola.Any())
                {
                    sql += "AND p.esc_codigo IN @Escolas ";
                }

                if (lsTurma.Any())
                {
                    sql += "AND p.tur_codigo IN @Turmas ";
                }

                sql += "ORDER BY p.esc_codigo, p.tur_codigo";

                resultado = conn.Query<Participacao>(
                                        sql: sql,
                                        param: new
                                        {
                                            Edicao = new DbString() { Value = Edicao, IsAnsi = true, Length = 10 },
                                            AnoEscolar = new DbString() { Value = AnoEscolar, IsAnsi = true, Length = 3 },
                                            Escolas = lsEscola,
                                            Turmas = lsTurma
                                        }).ToList();
            }

            return resultado;
        }
    }
}
