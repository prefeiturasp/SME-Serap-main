using Dapper;
using GestaoAvaliacao.Entities;
using GestaoAvaliacao.IRepository;
using GestaoAvaliacao.Repository.Context;
using System;
using System.Data;
using System.Linq;

namespace GestaoAvaliacao.Repository
{
    public class AnswerSheetBatchLogRepository : ConnectionReadOnly, IAnswerSheetBatchLogRepository
	{
		#region Read

        public AnswerSheetBatchLog Get(long id)
        {
            using (IDbConnection cn = Connection)
            {
                cn.Open();
                var sql = @"SELECT [Id]
                                  ,[AnswerSheetBatchFile_Id]
                                  ,[Situation]
                                  ,[Description]
                                  ,[CreateDate]
                                  ,[UpdateDate]
                                  ,[State]
                                  ,[File_Id] " +
                           "FROM [AnswerSheetBatchLog] WITH (NOLOCK) " +
                           "WHERE [Id] = @id ";

                var result = cn.Query<AnswerSheetBatchLog>(sql, new { id = id }).FirstOrDefault();

                return result;
            }
        }

        public AnswerSheetBatchLog GetByBatchFile_Id(long id)
        {
            using (IDbConnection cn = Connection)
            {
                cn.Open();
                var sql = @"SELECT [Id]
                                  ,[AnswerSheetBatchFile_Id]
                                  ,[Situation]
                                  ,[Description]
                                  ,[CreateDate]
                                  ,[UpdateDate]
                                  ,[State]
                                  ,[File_Id] " +
                           "FROM [AnswerSheetBatchLog] WITH (NOLOCK) " +
                           "WHERE [AnswerSheetBatchFile_Id] = @id ";

                var result = cn.Query<AnswerSheetBatchLog>(sql, new { id = id }).FirstOrDefault();

                return result;
            }
        }

		#endregion

        #region Write

        public AnswerSheetBatchLog Save(AnswerSheetBatchLog entity)
        {
            using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                GestaoAvaliacaoContext.AnswerSheetBatchLog.Add(entity);
                GestaoAvaliacaoContext.SaveChanges();

                return entity;
            }
        }

        public void Update(AnswerSheetBatchLog entity)
        {
            using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                AnswerSheetBatchLog _entity = GestaoAvaliacaoContext.AnswerSheetBatchLog.FirstOrDefault(a => a.Id == entity.Id);

                if (!entity.Situation.Equals(_entity.Situation))
                    _entity.Situation = entity.Situation;

                if (string.IsNullOrEmpty(_entity.Description) || !entity.Description.Equals(_entity.Description))
                    _entity.Description = entity.Description;

                if (!entity.File_Id.Equals(_entity.File_Id))
                    _entity.File_Id = entity.File_Id;

                _entity.UpdateDate = DateTime.Now;

                GestaoAvaliacaoContext.Entry(_entity).State = System.Data.Entity.EntityState.Modified;
                GestaoAvaliacaoContext.SaveChanges();
            }
        }

        #endregion
	}
}
