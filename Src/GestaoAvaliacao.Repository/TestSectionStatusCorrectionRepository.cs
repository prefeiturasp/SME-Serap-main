using Dapper;
using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Entities.DTO;
using GestaoAvaliacao.Entities.Enumerator;
using GestaoAvaliacao.IRepository;
using GestaoAvaliacao.Repository.Context;
using GestaoAvaliacao.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Repository
{
    public class TestSectionStatusCorrectionRepository : ConnectionReadOnly, ITestSectionStatusCorrectionRepository
    {
        #region Write
        public TestSectionStatusCorrection Save(TestSectionStatusCorrection entity)
        {

            using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                DateTime dateNow = DateTime.Now;

                entity.CreateDate = dateNow;
                entity.UpdateDate = dateNow;
                entity.State = (byte)EnumState.ativo;

                GestaoAvaliacaoContext.TestSectionStatusCorrection.Add(entity);
                GestaoAvaliacaoContext.SaveChanges();

                return entity;
            }
            
        }

        public TestSectionStatusCorrection Update(TestSectionStatusCorrection entity)
        {

            using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                TestSectionStatusCorrection cadastred = GestaoAvaliacaoContext.TestSectionStatusCorrection.FirstOrDefault(a => a.Id == entity.Id);

                DateTime dateNow = DateTime.Now;

                entity.UpdateDate = dateNow;
                cadastred.StatusCorrection = entity.StatusCorrection;

                GestaoAvaliacaoContext.Entry(cadastred).State = System.Data.Entity.EntityState.Modified;
                GestaoAvaliacaoContext.SaveChanges();

                return entity;
            }            
        }

        public async Task<TestSectionStatusCorrection> UpdateAsync(TestSectionStatusCorrection entity)
        {
            using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                entity.UpdateDate = DateTime.Now;
                GestaoAvaliacaoContext.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                await GestaoAvaliacaoContext.SaveChangesAsync();

                return entity;
            }
        }

        public TestSectionStatusCorrection SetStatusCorrectionUpdate(TestSectionStatusCorrection entity)
        {

            using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                TestSectionStatusCorrection cadastred = GestaoAvaliacaoContext.TestSectionStatusCorrection.FirstOrDefault(a => a.Test_Id == entity.Test_Id &&
                    a.tur_id == entity.tur_id);

                DateTime dateNow = DateTime.Now;

                entity.UpdateDate = dateNow;
                cadastred.StatusCorrection = entity.StatusCorrection;

                GestaoAvaliacaoContext.Entry(cadastred).State = System.Data.Entity.EntityState.Modified;
                GestaoAvaliacaoContext.SaveChanges();

                return entity;
            }
            
        }
        #endregion

        #region Read
        public TestSectionStatusCorrection Get(long Test_Id, long tur_id)
        {
            var sql = new StringBuilder("SELECT Id, Test_Id, tur_id, StatusCorrection, CreateDate, UpdateDate, State ");
            sql.Append("FROM TestSectionStatusCorrection (NOLOCK) ");
            sql.Append("WHERE Test_Id = @Test_Id AND tur_id = @tur_id ");

            using (IDbConnection cn = Connection)
            {
                cn.Open();
                return cn.Query<TestSectionStatusCorrection>(sql.ToString(), new { Test_Id = Test_Id, tur_id = tur_id }).FirstOrDefault();
            }
        }

        public async Task<TestSectionStatusCorrection> GetAsync(long test_Id, long tur_id)
        {
            var sql = @"SELECT TOP 1 Id, Test_Id, tur_id, StatusCorrection, CreateDate, UpdateDate, State 
                      FROM TestSectionStatusCorrection (NOLOCK)
                      WHERE Test_Id = @test_Id AND tur_id = @tur_id ";

            using (IDbConnection cn = Connection)
            {
                cn.Open();
                var parametros = new { test_Id, tur_id };
                var result = await cn.QueryAsync<TestSectionStatusCorrection>(sql, parametros);
                return result.FirstOrDefault();
            }
        }

        public IEnumerable<TestSectionStatusCorrection> GetByTest(long Test_Id)
        {
            var sql = @"SELECT Tssc.Id, Tssc.Test_Id, Tssc.tur_id, Tssc.StatusCorrection, Tssc.CreateDate, Tssc.UpdateDate, Tssc.State, UadSuperior.uad_id AS idDRE, UadSuperior.uad_nome AS DRE, Esc.esc_id, Esc.esc_nome, Tur.tur_id, Tur.tur_codigo
                        FROM TestSectionStatusCorrection AS Tssc WITH(NOLOCK) 
	                        INNER JOIN SGP_TUR_Turma AS Tur WITH(NOLOCK) ON Tssc.tur_id = Tur.tur_id
	                        INNER JOIN SGP_ESC_Escola AS Esc WITH(NOLOCK) ON Tur.esc_id = Esc.esc_id
	                        LEFT JOIN SGP_SYS_UnidadeAdministrativa AS UadSuperior WITH(NOLOCK) ON Esc.uad_idSuperiorGestao = UadSuperior.uad_id
                        WHERE Tssc.Test_Id = @id 
                        GROUP BY Tssc.Id, Tssc.Test_Id, Tssc.tur_id, Tssc.StatusCorrection, Tssc.CreateDate, Tssc.UpdateDate, Tssc.State, UadSuperior.uad_id, UadSuperior.uad_nome, Esc.esc_id, Esc.esc_nome, Tur.tur_id, Tur.tur_codigo";

            using (IDbConnection cn = Connection)
            {
                cn.Open();

                return cn.Query<TestSectionStatusCorrection>(sql.ToString(), new { id = Test_Id });
            }
        }

        public IEnumerable<TestSectionStatusCorrection> GetBySchool(long Test_Id, int esc_id)
        {
            var sql = new StringBuilder("SELECT t.Id, t.Test_Id, t.tur_id, t.StatusCorrection, t.CreateDate, t.UpdateDate, t.State ");
            sql.Append("FROM TestSectionStatusCorrection t (NOLOCK) ");
            sql.Append("INNER JOIN SGP_TUR_Turma tur (NOLOCK) on t.tur_id = tur.tur_id ");
            sql.Append("WHERE Test_Id = @Test_Id AND tur.esc_id = @esc_id");

            using (IDbConnection cn = Connection)
            {
                cn.Open();

                return cn.Query<TestSectionStatusCorrection>(sql.ToString(), new { Test_Id = Test_Id, esc_id = esc_id });
            }
        }
        public IEnumerable<TestStatsEntitiesDTO> GetFinalizedEntities(long? Test_Id, string Year, Guid? uad_id, int? esc_id, long? tur_id, DateTime? FinalizationDate)
        {
            string tne_id = string.Empty, crp_ordem = string.Empty;

            var sql = new StringBuilder();
            sql.AppendLine("SELECT tur.tur_id, esc.esc_id, esc.uad_id, esc.uad_idSuperiorGestao, tssc.Test_Id");
            sql.AppendLine("FROM TestSectionStatusCorrection tssc WITH(NOLOCK)");
            sql.AppendLine("INNER JOIN SGP_TUR_Turma tur WITH(NOLOCK) ON tur.tur_id = tssc.tur_id");
            sql.AppendLine("INNER JOIN SGP_TUR_TurmaTipoCurriculoPeriodo ttcp WITH(NOLOCK) ON ttcp.tur_id = tur.tur_id");
            sql.AppendLine("INNER JOIN SGP_ESC_Escola esc WITH(NOLOCK) ON esc.esc_id = tur.esc_id");
            sql.AppendLine("WHERE (tssc.StatusCorrection = @Success OR tssc.StatusCorrection = @PartialSuccess)");

            if (Test_Id.HasValue)
                sql.AppendLine("AND Test_Id = @Test_Id");
            if (uad_id.HasValue && uad_id.Value != Guid.Empty)
                sql.AppendLine("AND uad.uad_id = @uad_id");
            if (esc_id.HasValue && esc_id.Value > 0)
                sql.AppendLine("AND esc.esc_id = @esc_id");
            if (tur_id.HasValue && tur_id.Value > 0)
                sql.AppendLine("AND tur.tur_id = @tur_id");
            if (!string.IsNullOrEmpty(Year))
            {
                sql.AppendLine("AND ttcp.tne_id = @tne_id AND ttcp.crp_ordem = @crp_ordem");
                var val = Year.Split('_');
                tne_id = val[0];
                crp_ordem = val[1];
            }
            if (FinalizationDate.HasValue)
                sql.AppendLine("AND tssc.UpdateDate > @FinalizationDate");
            using (IDbConnection cn = Connection)
            {
                cn.Open();

                return cn.Query<TestStatsEntitiesDTO>(sql.ToString(),
                    new
                    {
                        Test_Id = Test_Id,
                        Success = EnumStatusCorrection.Success,
                        PartialSuccess = EnumStatusCorrection.PartialSuccess,
                        FinalizationDate = FinalizationDate,
                        uad_id = uad_id,
                        esc_id = esc_id,
                        tur_id = tur_id,
                        tne_id = tne_id,
                        crp_ordem = crp_ordem
                    });
            }
        }
        public List<TestSectionStatusCorrection> GetAll()
        {
            var sql = new StringBuilder("SELECT Test_Id, tur_id ");
            sql.Append("FROM TestSectionStatusCorrection (NOLOCK) ");
            sql.Append("WHERE (StatusCorrection = 2 OR StatusCorrection = 3) AND State <> 3 ");
            sql.Append("GROUP BY Test_Id, tur_id ");

            using (IDbConnection cn = Connection)
            {
                cn.Open();

                return cn.Query<TestSectionStatusCorrection>(sql.ToString()).ToList();

            }
        }

        public IEnumerable<FinalizedTestYearDTO> GetSectionsToCalculate(DateTime? PartialDate)
        {

            var sql = new StringBuilder();

            //Primeiro verifica as escolas e ano da uma prova que tiveram turmas finalizadas após a ultima execução do serviço
            sql.AppendLine("WITH E AS");
            sql.AppendLine("(");
            sql.AppendLine("SELECT DISTINCT tssc.Test_Id, ttcp.crp_ordem, tur.esc_id, esc.uad_idSuperiorGestao, ttcp.tne_id");
            sql.AppendLine("FROM TestSectionStatusCorrection tssc");
            sql.AppendLine("INNER JOIN SGP_TUR_Turma tur ON tssc.tur_id = tur.tur_id");
            sql.AppendLine("INNER JOIN SGP_ESC_Escola esc ON esc.esc_id = tur.esc_id");
            sql.AppendLine("INNER JOIN Test t ON t.Id = tssc.Test_Id");
            sql.AppendLine("INNER JOIN TestType tt ON tt.Id = t.TestType_Id");
            sql.AppendLine("INNER JOIN SGP_TUR_TurmaTipoCurriculoPeriodo ttcp ON ttcp.tur_id = tssc.tur_id");
            sql.AppendLine("WHERE(tssc.StatusCorrection = @Success OR tssc.StatusCorrection = @PartialSuccess)");

            if (PartialDate.HasValue)
                sql.AppendLine("AND ttcp.UpdateDate >= @PartialDate");

            sql.AppendLine(")");

            //Depois seleciona todas as turmas daquela prova para aquela escola e ano
            sql.AppendLine("SELECT tssc.Test_Id, E.esc_id, E.uad_idSuperiorGestao, ttcp.crp_ordem, ttcp.tne_id, tur.tur_id ");
            sql.AppendLine("FROM E");
            sql.AppendLine("INNER JOIN SGP_TUR_Turma tur ON tur.esc_id = E.esc_id");
            sql.AppendLine("INNER JOIN TestSectionStatusCorrection tssc ON tur.tur_id = tssc.tur_id AND tssc.Test_Id = E.Test_Id");
            sql.AppendLine("INNER JOIN SGP_TUR_TurmaTipoCurriculoPeriodo ttcp ON ttcp.tur_id = tur.tur_id AND ttcp.crp_ordem = E.crp_ordem");
            sql.AppendLine("AND ttcp.tne_id = E.tne_id");
            sql.AppendLine("WHERE(tssc.StatusCorrection = @Success OR tssc.StatusCorrection = @PartialSuccess)");

            using (IDbConnection cn = Connection)
            {
                cn.Open();

                return cn.Query<FinalizedTestYearDTO>(sql.ToString(),
                new
                {
                    Success = EnumStatusCorrection.Success,
                    PartialSuccess = EnumStatusCorrection.PartialSuccess,
                    PartialDate = PartialDate
                });
            }
        }
        #endregion
    }
}
