using Dapper;
using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Entities.Enumerator;
using GestaoAvaliacao.IRepository;
using GestaoAvaliacao.Repository.Context;
using GestaoAvaliacao.Util;
using System;
using System.Data;
using System.Linq;
using System.Text;

namespace GestaoAvaliacao.Repository
{
    public class AnswerSheetBatchRepository : ConnectionReadOnly, IAnswerSheetBatchRepository
    {
        private readonly IAnswerSheetBatchFilesRepository answerSheetBatchFilesRepository;

        public AnswerSheetBatchRepository(IAnswerSheetBatchFilesRepository answerSheetBatchFilesRepository)
        {
            this.answerSheetBatchFilesRepository = answerSheetBatchFilesRepository;
        }

        #region Read

        public AnswerSheetBatch Get(long id)
        {
            using (IDbConnection cn = Connection)
            {
                cn.Open();
                var sql = @"SELECT [Id]
							  ,[Description]
							  ,[Test_Id]
							  ,[SupAdmUnit_Id]
							  ,[School_Id]
							  ,[Section_Id]
							  ,[CreatedBy_Id]
							  ,[Processing]
							  ,[CreateDate]
							  ,[UpdateDate]
							  ,[State]
							  ,[BatchType]
							  ,[OwnerEntity] " +
                           "FROM [AnswerSheetBatch] WITH (NOLOCK) " +
                           "WHERE [Id] = @id ";

                AnswerSheetBatch result = cn.Query<AnswerSheetBatch>(sql, new { id = id }).FirstOrDefault();
                result.AnswerSheetBatchFiles.AddRange(answerSheetBatchFilesRepository.GetFiles(id, false));

                return result;
            }
        }

        public AnswerSheetBatch GetSimple(long id)
        {
            using (IDbConnection cn = Connection)
            {
                cn.Open();
                var sql = @"SELECT [Id]
							  ,[Description]
							  ,[Test_Id]
							  ,[SupAdmUnit_Id]
							  ,[School_Id]
							  ,[Section_Id]
							  ,[CreatedBy_Id]
							  ,[Processing]
							  ,[CreateDate]
							  ,[UpdateDate]
							  ,[State]
							  ,[BatchType]
							  ,[OwnerEntity] " +
                           "FROM [AnswerSheetBatch] WITH (NOLOCK) " +
                           "WHERE [Id] = @id ";

                AnswerSheetBatch result = cn.Query<AnswerSheetBatch>(sql, new { id = id }).FirstOrDefault();
                return result;
            }
        }

        public AnswerSheetBatch Find(AnswerSheetBatchFilter filter)
        {
            StringBuilder sql = new StringBuilder("SELECT B.[Id],B.[Description],B.[Test_Id],B.[SupAdmUnit_Id],B.[School_Id] ");
            sql.Append(",B.[Section_Id],B.[CreatedBy_Id],B.[Processing],B.[CreateDate],B.[UpdateDate] ");
            sql.Append(",B.[State],B.[BatchType],B.[OwnerEntity] ");
            sql.Append("FROM [AnswerSheetBatch] B WITH (NOLOCK) ");
            sql.Append("WHERE B.[State] <> @State ");

            if (filter.BatchId > 0)
                sql.Append("AND B.[Id] = @Id ");
            else
            {
                if (filter.TestId > 0)
                    sql.Append("AND B.[Test_Id] = @TestId ");

                if (filter.SectionId > 0)
                {
                    sql.AppendFormat("AND B.[Section_Id] = @SectionId AND B.[School_Id] = @SchoolId AND B.[OwnerEntity] = {0} ", (byte)EnumAnswerSheetBatchOwner.Section);
                }
                else if (filter.SchoolId > 0)
                {
                    sql.AppendFormat("AND B.[School_Id] = @SchoolId AND B.[OwnerEntity] = {0} ", (byte)EnumAnswerSheetBatchOwner.School);

                    if (filter.CoreVisionId == (int)EnumSYS_Visao.Individual)
                    {
                        sql.Append("AND B.[CreatedBy_Id] = @UserId ");
                    }
                }
                else
                {
                    sql.AppendFormat("AND B.[OwnerEntity] = {0} ", (byte)EnumAnswerSheetBatchOwner.Test);
                }
            }

            using (IDbConnection cn = Connection)
            {
                cn.Open();

                var parameters = new
                {
                    UserId = filter.UserId,
                    State = (Byte)EnumState.excluido,
                    Id = filter.BatchId,
                    TestId = filter.TestId,
                    SchoolId = filter.SchoolId,
                    SectionId = filter.SectionId
                };

                var result = cn.Query<AnswerSheetBatch>(sql.ToString(), parameters).FirstOrDefault();

                return result;
            }
        }

        public AnswerSheetBatchResult GetSchoolSectionInformation(int SchoolId, long SectionId)
        {
            StringBuilder sql = new StringBuilder("SELECT UAD.[uad_id] AS SupAdmUnitId, UAD.[uad_nome] AS SupAdmUnitName, UAD.[uad_sigla] AS SupAdmUnitCode, E.[esc_id] AS SchoolId, E.[esc_nome] AS SchoolName ");
            if (SectionId > 0)
                sql.Append(", T.[tur_codigo] AS SectionCode, T.[tur_codigo] + ' - ' + TN.[ttn_nome] AS SectionName ");

            sql.Append("FROM [SGP_ESC_Escola] E WITH (NOLOCK) ");
            sql.Append("INNER JOIN [SGP_SYS_UnidadeAdministrativa] UAD WITH (NOLOCK) ON UAD.[uad_id] = E.[uad_idSuperiorGestao] AND UAD.[uad_situacao] = 1 ");
            if (SectionId > 0)
            {
                sql.Append("INNER JOIN [SGP_TUR_Turma] T WITH (NOLOCK) ON T.[esc_id] = E.[esc_id] AND T.[tur_situacao] = 1 ");
                sql.Append("INNER JOIN [SGP_ACA_TipoTurno] TN WITH (NOLOCK) ON TN.[ttn_id] = T.[ttn_id] AND TN.[ttn_situacao] = 1 ");
            }

            sql.Append("WHERE ");

            if (SectionId > 0)
                sql.Append("E.[esc_id] = @School_Id AND T.[tur_id] = @Section_Id ");
            else if (SchoolId > 0)
                sql.Append("E.[esc_id] = @School_Id ");

            using (IDbConnection cn = Connection)
            {
                cn.Open();

                return cn.Query<AnswerSheetBatchResult>(sql.ToString(), new { School_Id = SchoolId, Section_Id = SectionId }).FirstOrDefault();
            }
        }

        public long GetStudentId(long SectionId, int? mtu_numeroChamada, long? alu_id)
        {
            StringBuilder sql = new StringBuilder("SELECT [alu_id] ");
            sql.Append("FROM [SGP_MTR_MatriculaTurma] WITH (NOLOCK) ");
            sql.Append("WHERE [tur_id] = @tur_id AND [mtu_situacao] = 1 ");

            if (mtu_numeroChamada > 0)
                sql.Append("AND [mtu_numeroChamada] = @mtu_numeroChamada ");
            if (alu_id > 0)
                sql.Append("AND [alu_id] = @alu_id ");

            using (IDbConnection cn = Connection)
            {
                cn.Open();

                return cn.Query<long>(sql.ToString(), new { tur_id = SectionId, mtu_numeroChamada = mtu_numeroChamada, alu_id = alu_id }).FirstOrDefault();
            }
        }

        #endregion

        #region Write

        public AnswerSheetBatch Save(AnswerSheetBatch entity)
        {
            using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                GestaoAvaliacaoContext.AnswerSheetBatch.Add(entity);
                GestaoAvaliacaoContext.SaveChanges();

                return entity;
            }
        }

        public void Update(AnswerSheetBatch entity)
        {
            using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                AnswerSheetBatch _entity = GestaoAvaliacaoContext.AnswerSheetBatch.FirstOrDefault(a => a.Id == entity.Id);

                if (!_entity.Processing.Equals(entity.Processing))
                    _entity.Processing = entity.Processing;

                _entity.UpdateDate = DateTime.Now;

                GestaoAvaliacaoContext.Entry(_entity).State = System.Data.Entity.EntityState.Modified;
                GestaoAvaliacaoContext.SaveChanges();
            }
        }

        public void UpdateOwnerEntities(AnswerSheetBatch entity)
        {
            using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                AnswerSheetBatch _entity = GestaoAvaliacaoContext.AnswerSheetBatch.FirstOrDefault(a => a.Id == entity.Id);

                if (!_entity.Section_Id.Equals(entity.Section_Id))
                    _entity.Section_Id = entity.Section_Id;

                if (!_entity.School_Id.Equals(entity.School_Id))
                    _entity.School_Id = entity.School_Id;

                if (!_entity.SupAdmUnit_Id.Equals(entity.SupAdmUnit_Id))
                    _entity.SupAdmUnit_Id = entity.SupAdmUnit_Id;

                _entity.UpdateDate = DateTime.Now;

                GestaoAvaliacaoContext.Entry(_entity).State = System.Data.Entity.EntityState.Modified;
                GestaoAvaliacaoContext.SaveChanges();
            }
        }

        #endregion
    }
}
