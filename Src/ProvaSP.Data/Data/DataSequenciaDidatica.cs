using Dapper;
using ProvaSP.Data.Funcionalidades;
using ProvaSP.Model.Entidades;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace ProvaSP.Data.Data
{
    public class DataSequenciaDidatica
    {
        public static List<Corte> SelecionarCorte()
        {
            using (var conn = new SqlConnection(StringsConexao.ProvaSP))
            {
                return conn.Query<Corte>(
                    sql: @"SELECT c.CorteId, c.Nome
                           FROM Corte c WITH(NOLOCK)
                           ORDER BY c.CorteId"
                    ).ToList();
            }
        }

        public static SequenciaDidatica BuscarSequenciaDidatica(string edicao, int anoEscolar, int areaConhecimentoId, int corteId)
        {
            using (var conn = new SqlConnection(StringsConexao.ProvaSP))
            {
                return conn.Query<SequenciaDidatica>(
                    sql: @"SELECT Edicao, AnoEscolar, AreaConhecimentoId, CorteId, Titulo, Texto, Link
                           FROM SequenciaDidatica WITH(NOLOCK) 
                           WHERE Edicao = @edicao AND AnoEscolar = @anoEscolar AND AreaConhecimentoId = @areaConhecimentoId AND CorteId = @corteId",
                            param: new
                            {
                                edicao,
                                anoEscolar,
                                areaConhecimentoId,
                                corteId
                            }).FirstOrDefault();
            }
        }

        public static bool SalvarSequenciaDidatica(SequenciaDidatica sequenciaDidatica)
        {
            using (var conn = new SqlConnection(StringsConexao.ProvaSP))
            {
                int ret = conn.Execute(@"IF EXISTS (SELECT * FROM SequenciaDidatica WITH(NOLOCK) WHERE Edicao = @Edicao AND AnoEscolar = @AnoEscolar AND AreaConhecimentoId = @AreaConhecimentoId AND CorteId = @CorteId) 
                                        BEGIN
                                            UPDATE SequenciaDidatica 
                                            SET Titulo = @Titulo, Texto = @Texto, Link = @Link 
                                            WHERE Edicao = @Edicao AND AnoEscolar = @AnoEscolar AND AreaConhecimentoId = @AreaConhecimentoId AND CorteId = @CorteId
                                        END
                                        ELSE
                                        BEGIN
                                            INSERT INTO SequenciaDidatica (Edicao, AnoEscolar, AreaConhecimentoId, CorteId, Titulo, Texto, Link)
                                            VALUES (@Edicao, @AnoEscolar, @AreaConhecimentoId, @CorteId, @Titulo, @Texto, @Link)
                                        END",
                            param: new
                            {
                                sequenciaDidatica.Edicao,
                                sequenciaDidatica.AnoEscolar,
                                sequenciaDidatica.AreaConhecimentoId,
                                sequenciaDidatica.CorteId,
                                sequenciaDidatica.Titulo,
                                sequenciaDidatica.Texto,
                                sequenciaDidatica.Link
                            });
                return ret > 0;
            }
        }

        public static List<Corte> SelecionarSequenciaDidatica(string edicao, int cicloId, int areaConhecimentoId)
        {
            using (var conn = new SqlConnection(StringsConexao.ProvaSP))
            {
                List<SequenciaDidatica> sequenciaDidaticas = conn.Query<SequenciaDidatica>(
                    sql: @"SELECT s.Edicao, s.AnoEscolar, s.AreaConhecimentoId, s.CorteId, s.Titulo, s.Texto, s.Link, c.Nome AS CorteNome
                           FROM SequenciaDidatica s WITH(NOLOCK) 
                                INNER JOIN Corte c WITH(NOLOCK) ON c.CorteId = s.CorteId
                                INNER JOIN CicloAnoEscolar ca WITH(NOLOCK) ON ca.AnoEscolar = s.AnoEscolar
                           WHERE s.Edicao = @edicao AND ca.CicloId = @cicloId AND s.AreaConhecimentoId = @areaConhecimentoId
                           ORDER BY c.CorteId, s.AnoEscolar",
                            param: new
                            {
                                edicao,
                                cicloId,
                                areaConhecimentoId
                            }).ToList();

                return sequenciaDidaticas.GroupBy(g => new { g.CorteId, g.CorteNome }).Select(p => new Corte
                {
                    CorteId = p.Key.CorteId,
                    Nome = p.Key.CorteNome,
                    SequenciasDidaticas = p.Select(q => new SequenciaDidatica
                    {
                        Edicao = q.Edicao,
                        AnoEscolar = q.AnoEscolar,
                        AreaConhecimentoId = q.AreaConhecimentoId,
                        CorteId = q.CorteId,
                        CorteNome = q.CorteNome,
                        Titulo = q.Titulo,
                        Texto = q.Texto,
                        Link = q.Link
                    }).OrderBy(o => o.AnoEscolar).ToList()
                }).ToList();
            }
        }
    }
}
