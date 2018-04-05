using Dapper;
using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Entities.Enumerator;
using GestaoAvaliacao.IRepository;
using GestaoAvaliacao.Repository.Context;
using GestaoAvaliacao.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace GestaoAvaliacao.Repository
{
    public class AbsenceReasonRepository : ConnectionReadOnly, IAbsenceReasonRepository
	{
		#region Read

		public IEnumerable<AbsenceReason> Load(ref Pager pager, Guid EntityId)
		{
			using (IDbConnection cn = Connection)
			{
				cn.Open();
				var sql = @"WITH NumberedAbsenceReason AS " +
							"( " +
                               "SELECT Id, Description, AllowRetry, IsDefault, " +
							   "ROW_NUMBER() OVER (ORDER BY Description) AS RowNumber " +
							   "FROM AbsenceReason " +
							   "WHERE State  = @state " +
							   "AND EntityId = @entityid " +
							") " +
                           "SELECT Id, Description, AllowRetry, IsDefault " +
						   "FROM NumberedAbsenceReason " +
						   "WHERE RowNumber > ( @pageSize * @page ) " +
						   "AND RowNumber <= ( ( @page + 1 ) * @pageSize ) " +
						   "ORDER BY RowNumber";

				var countSql = @"SELECT COUNT(id) " +
								"FROM AbsenceReason " +
								"WHERE State = @state " +
								"AND EntityId = @entityid";

				var absenceReason = cn.Query<AbsenceReason>(sql, new { state = (Byte)EnumState.ativo, entityid = EntityId, pageSize = pager.PageSize, page = pager.CurrentPage });
				var count = (int)cn.ExecuteScalar(countSql, new { state = (Byte)EnumState.ativo, entityid = EntityId });

				pager.SetTotalPages((int)Math.Ceiling(absenceReason.Count() / (double)pager.PageSize));
				pager.SetTotalItens(count);

				return absenceReason;
			}
		}

		public IEnumerable<AbsenceReason> Search(String search, ref Pager pager, Guid EntityId)
		{
			using (IDbConnection cn = Connection)
			{
				cn.Open();
				var sql = @"WITH NumberedAbsenceReason AS " +
							"( " +
                               "SELECT Id, Description, AllowRetry, IsDefault, " +
							   "ROW_NUMBER() OVER (ORDER BY Description) AS RowNumber " +
							   "FROM AbsenceReason " +
							   "WHERE State  = @state " +
							   "AND EntityId = @entityid " +
							   "AND (@search IS NULL OR Description Like '%' + @search + '%') " +
							") " +
                           "SELECT Id, Description, AllowRetry, IsDefault " +
						   "FROM NumberedAbsenceReason " +
						   "WHERE RowNumber > ( @pageSize * @page ) " +
						   "AND RowNumber <= ( ( @page + 1 ) * @pageSize ) " +
						   "ORDER BY RowNumber";

				var countSql = @"SELECT COUNT(id) " +
								"FROM AbsenceReason " +
								"WHERE State = @state " +
								"AND (@search IS NULL OR Description Like '%' + @search + '%') " +
								"AND EntityId = @entityid";

				var absenceReason = cn.Query<AbsenceReason>(sql, new { state = (Byte)EnumState.ativo, entityid = EntityId, pageSize = pager.PageSize, page = pager.CurrentPage, search = search });
				var count = (int)cn.ExecuteScalar(countSql, new { state = (Byte)EnumState.ativo, entityid = EntityId, search = search });

				pager.SetTotalPages((int)Math.Ceiling(absenceReason.Count() / (double)pager.PageSize));
				pager.SetTotalItens(count);

				return absenceReason;
			}
		}

		public AbsenceReason Get(long id, Guid EntityId)
		{
			using (IDbConnection cn = Connection)
			{
				cn.Open();
				var sql = @"SELECT Id, Description, AllowRetry, IsDefault " +
						   "FROM AbsenceReason " +
						   "WHERE Id = @id " +
						   "AND EntityId = @entityid";

				var absenceReason = cn.Query<AbsenceReason>(sql, new { id = id, entityid = EntityId }).FirstOrDefault();

				return absenceReason;
			}
		}

        public AbsenceReason GetDefault(Guid EntityId)
        {
            #region Consulta
            var sql = new StringBuilder("SELECT Id, Description, AllowRetry, IsDefault ");
            sql.AppendLine("FROM AbsenceReason WITH (NOLOCK) ");
            sql.AppendLine("WHERE EntityId = @entityid AND IsDefault = 1");
            #endregion

            using (IDbConnection cn = Connection)
            {
                cn.Open();
                return cn.Query<AbsenceReason>(sql.ToString(), new { entityid = EntityId }).FirstOrDefault();
            }
        }

        public List<AbsenceReason> GetAll(Guid EntityId)
        {
            #region Consulta
            var sql = new StringBuilder("SELECT Id, Description ");
            sql.AppendLine("FROM AbsenceReason WITH (NOLOCK) ");
            sql.AppendLine("WHERE EntityId = @entityid ");
            #endregion

            using (IDbConnection cn = Connection)
            {
                cn.Open();
                return cn.Query<AbsenceReason>(sql.ToString(), new { entityid = EntityId }).ToList();
            }
        }

        public IEnumerable<AbsenceReason> Get(Guid EntityId)
		{
			var sql = new StringBuilder();
			sql.AppendLine("SELECT Id, Description, AllowRetry, IsDefault ");
			sql.AppendLine("FROM AbsenceReason");
			sql.AppendLine("WHERE EntityId = @entityid");
			sql.AppendLine("AND State = @state");

			using (IDbConnection cn = Connection)
			{
				cn.Open();

				var absenceReason = cn.Query<AbsenceReason>(sql.ToString(), new { entityid = EntityId, state = (byte)EnumState.ativo });

				return absenceReason;
			}
		}

		public bool ExistsDescriptionNamed(String Description, Guid ent_id)
		{
			using (IDbConnection cn = Connection)
			{
				cn.Open();
				var sql = @"SELECT COUNT(Id) " +
						   "FROM AbsenceReason " +
						   "WHERE Description COLLATE Latin1_General_CI_AS = @Description " +
						   "AND EntityId = @entityid " +
						   "AND State = @state ";

				var count = (int)cn.ExecuteScalar(sql, new { Description = Description, entityid = ent_id, state = (Byte)EnumState.ativo });

				return count > 0;
			}
		}

		public bool ExistsDescriptionNamedAlter(String Description, int id, Guid ent_id)
		{
			using (IDbConnection cn = Connection)
			{
				cn.Open();
				var sql = @"SELECT COUNT(Id) " +
						   "FROM AbsenceReason " +
						   "WHERE Description COLLATE Latin1_General_CI_AS = @Description " +
						   "AND EntityId = @entityid " +
						   "AND State = @state " +
						   "AND Id != @id ";

				var count = (int)cn.ExecuteScalar(sql, new { Description = Description, entityid = ent_id, state = (Byte)EnumState.ativo, id = id });
				return count > 0;
			}
		}

		#endregion

		#region Write

		public AbsenceReason Save(AbsenceReason entity)
		{
			using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
			{
				entity.CreateDate = DateTime.Now; 
				entity.UpdateDate = DateTime.Now; 
				entity.State = Convert.ToByte(EnumState.ativo);

				GestaoAvaliacaoContext.AbsenceReason.Add(entity);
				GestaoAvaliacaoContext.SaveChanges();

				return entity;
			}
		}

		public void Update(AbsenceReason entity)
		{
			using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
			{
				AbsenceReason absenceReason = GestaoAvaliacaoContext.AbsenceReason.FirstOrDefault(a => a.Id == entity.Id);

				if (!string.IsNullOrEmpty(entity.Description))
					absenceReason.Description = entity.Description;
				
				absenceReason.AllowRetry = entity.AllowRetry;
                absenceReason.IsDefault = entity.IsDefault;
				absenceReason.UpdateDate = DateTime.Now;

				GestaoAvaliacaoContext.Entry(absenceReason).State = System.Data.Entity.EntityState.Modified;
				GestaoAvaliacaoContext.SaveChanges();
			}
		}

		public void Delete(long id)
		{
			using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
			{
				AbsenceReason absenceReason = GestaoAvaliacaoContext.AbsenceReason.FirstOrDefault(a => a.Id == id);

				absenceReason.State = Convert.ToByte(EnumState.excluido);
				absenceReason.UpdateDate = DateTime.Now;

				GestaoAvaliacaoContext.Entry(absenceReason).State = System.Data.Entity.EntityState.Modified;
				GestaoAvaliacaoContext.SaveChanges();
			}
		}

        public void VerifyDefault(Guid EntityId)
        {
            var sql = new StringBuilder("UPDATE AbsenceReason SET IsDefault = 0, UpdateDate = @updateDate ");
            sql.AppendLine("WHERE EntityId = @entityId AND IsDefault = 1 AND State = @state");

            using (IDbConnection cn = Connection)
            {
                cn.Open();

                cn.Execute(sql.ToString(),
                    new
                    {
                        entityId = EntityId,
                        updateDate = DateTime.Now,
                        state = Convert.ToByte(EnumState.ativo)
                    });
            }
        }

        #endregion
    }
}
