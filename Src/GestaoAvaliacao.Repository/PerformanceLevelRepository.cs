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

namespace GestaoAvaliacao.Repository
{
    public class PerformanceLevelRepository : ConnectionReadOnly, IPerformanceLevelRepository
	{
		#region Readyonly

		public IEnumerable<PerformanceLevel> GetAll(Guid EntityId)
		{
			using (IDbConnection cn = Connection)
			{
				cn.Open();
				var sql = @"SELECT Id, Description, Code " +
						   "FROM PerformanceLevel " +
						   "WHERE State = @state " +
						   "AND EntityId = @entityid " +
						   "ORDER BY Code ";

				var performanceLevel = cn.Query<PerformanceLevel>(sql, new { state = (Byte)EnumState.ativo, entityid = EntityId });

				return performanceLevel;
			}
		}

		public PerformanceLevel Get(long id)
		{
			using (IDbConnection cn = Connection)
			{
				cn.Open();
				var sql = @"SELECT Id, Description, Code " +
						   "FROM PerformanceLevel " +
						   "WHERE Id = @id ";

				var performanceLevel = cn.Query<PerformanceLevel>(sql, new { id = id }).FirstOrDefault();

				return performanceLevel;
			}
		}

		public IEnumerable<PerformanceLevel> Search(String search, ref Pager pager, Guid EntityId)
		{
			using (IDbConnection cn = Connection)
			{
				cn.Open();
				var sql = @"WITH NumberedPerformanceLevel AS " +
							"( " +
							   "SELECT Id, Description, Code, " +
							   "ROW_NUMBER() OVER (ORDER BY Code) AS RowNumber " +
							   "FROM PerformanceLevel " +
							   "WHERE State  = @state " +
							   "AND EntityId = @entityid " +
							   "AND (@search IS NULL OR Description Like '%' + @search + '%') " +
							") " +
						   "SELECT Id, Description, Code " +
						   "FROM NumberedPerformanceLevel " +
						   "WHERE RowNumber > ( @pageSize * @page ) " +
						   "AND RowNumber <= ( ( @page + 1 ) * @pageSize ) " +
						   "ORDER BY RowNumber";

				var countSql = @"SELECT COUNT(id) " +
								"FROM PerformanceLevel " +
								"WHERE State = @state " +
								"AND (@search IS NULL OR Description Like '%' + @search + '%') " +
								"AND EntityId = @entityid";

				var performanceLevel = cn.Query<PerformanceLevel>(sql, new { state = (Byte)EnumState.ativo, entityid = EntityId, pageSize = pager.PageSize, page = pager.CurrentPage, search = search });
				var count = (int)cn.ExecuteScalar(countSql, new { state = (Byte)EnumState.ativo, entityid = EntityId, search = search });

				pager.SetTotalPages((int)Math.Ceiling(performanceLevel.Count() / (double)pager.PageSize));
				pager.SetTotalItens(count);

				return performanceLevel;
			}
		}

		public IEnumerable<PerformanceLevel> Load(ref Pager pager, Guid EntityId)
		{
			using (IDbConnection cn = Connection)
			{
				cn.Open();
				var sql = @"WITH NumberedPerformanceLevel AS " +
							"( " +
							   "SELECT Id, Description, Code, " +
							   "ROW_NUMBER() OVER (ORDER BY Code) AS RowNumber " +
							   "FROM PerformanceLevel " +
							   "WHERE State  = @state " +
							   "AND EntityId = @entityid " +
							") " +
						   "SELECT Id, Description, Code " +
						   "FROM NumberedPerformanceLevel " +
						   "WHERE RowNumber > ( @pageSize * @page ) " +
						   "AND RowNumber <= ( ( @page + 1 ) * @pageSize ) " +
						   "ORDER BY RowNumber";

				var countSql = @"SELECT COUNT(id) " +
								"FROM PerformanceLevel " +
								"WHERE State = @state " +
								"AND EntityId = @entityid";

				var performanceLevel = cn.Query<PerformanceLevel>(sql, new { state = (Byte)EnumState.ativo, entityid = EntityId, pageSize = pager.PageSize, page = pager.CurrentPage });
				var count = (int)cn.ExecuteScalar(countSql, new { state = (Byte)EnumState.ativo, entityid = EntityId });

				pager.SetTotalPages((int)Math.Ceiling(performanceLevel.Count() / (double)pager.PageSize));
				pager.SetTotalItens(count);

				return performanceLevel;
			}
		}

		public IEnumerable<PerformanceLevel> LoadLevels(Guid EntityId)
		{
			using (IDbConnection cn = Connection)
			{
				cn.Open();
				var sql = @"SELECT Id, Description, Code " +
						   "FROM PerformanceLevel " +
						   "WHERE State = @state " +
						   "AND EntityId = @entityid " +
						   "ORDER BY Code ";

				var performanceLevel = cn.Query<PerformanceLevel>(sql, new { state = (Byte)EnumState.ativo, entityid = EntityId });

				return performanceLevel;
			}
		}

		public bool ExistsCode(string code, Guid ent_id)
		{
			using (IDbConnection cn = Connection)
			{
				cn.Open();
				var sql = @"SELECT COUNT(Id) " +
						   "FROM PerformanceLevel " +
						   "WHERE State = @state " +
						   "AND Code = @code " +
						   "AND EntityId = @entityid ";

				var count = (int)cn.ExecuteScalar(sql, new { code = code, state = (Byte)EnumState.ativo, entityid = ent_id });

				return count > 0;
			}
		}

		public bool ExistsDescription(string description, Guid ent_id)
		{
			using (IDbConnection cn = Connection)
			{
				cn.Open();
				var sql = @"SELECT COUNT(Id) " +
						   "FROM PerformanceLevel " +
						   "WHERE State = @state " +
						   "AND UPPER(Description) = UPPER(@description) " +
						   "AND EntityId = @entityid ";

				var count = (int)cn.ExecuteScalar(sql, new { description = description, state = (Byte)EnumState.ativo, entityid = ent_id });

				return count > 0;
			}
		}

		public bool ExistsCodeAlter(string code, long id, Guid ent_id)
		{
			using (IDbConnection cn = Connection)
			{
				cn.Open();
				var sql = @"SELECT COUNT(Id) " +
						   "FROM PerformanceLevel " +
						   "WHERE Code = @code " +
						   "AND EntityId = @entityid " +
						   "AND State = @state " +
						   "AND Id != @id ";

				var count = (int)cn.ExecuteScalar(sql, new { code = code, entityid = ent_id, state = (Byte)EnumState.ativo, id = id });

				return count > 0;
			}
		}

		public bool ExistsDescriptionAlter(string description, long id, Guid ent_id)
		{
			using (IDbConnection cn = Connection)
			{
				cn.Open();
				var sql = @"SELECT COUNT(Id) " +
						   "FROM PerformanceLevel " +
						   "WHERE UPPER(Description) = UPPER(@description) " +
						   "AND EntityId = @entityid " +
						   "AND State = @state " +
						   "AND Id != @id ";

				var count = (int)cn.ExecuteScalar(sql, new { description = description, entityid = ent_id, state = (Byte)EnumState.ativo, id = id });

				return count > 0;
			}
		}

		#endregion

		#region CRUD

		public PerformanceLevel Save(PerformanceLevel entity)
		{
			using (GestaoAvaliacaoContext gestaoAvaliacaoContext = new GestaoAvaliacaoContext())
			{
				DateTime dateNow = DateTime.Now;
				entity.CreateDate = dateNow;
				entity.UpdateDate = dateNow;
				entity.State = Convert.ToByte(EnumState.ativo);

				gestaoAvaliacaoContext.PerformanceLevel.Add(entity);
				gestaoAvaliacaoContext.SaveChanges();
			}
			return entity;
		}

		public void Update(PerformanceLevel entity)
		{
			using (GestaoAvaliacaoContext gestaoAvaliacaoContext = new GestaoAvaliacaoContext())
			{
				PerformanceLevel performanceLevel = gestaoAvaliacaoContext.PerformanceLevel.FirstOrDefault(il => il.Id == entity.Id);

				if (!string.IsNullOrEmpty(entity.Description))
					performanceLevel.Description = entity.Description;

				if (!string.IsNullOrEmpty(entity.Code))
					performanceLevel.Code = entity.Code;

				performanceLevel.UpdateDate = DateTime.Now;

				gestaoAvaliacaoContext.Entry(performanceLevel).State = System.Data.Entity.EntityState.Modified;

				gestaoAvaliacaoContext.SaveChanges();
			}
		}

		public void Delete(PerformanceLevel entity)
		{
			using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
			{
				PerformanceLevel performanceLevel = GestaoAvaliacaoContext.PerformanceLevel.FirstOrDefault(a => a.Id == entity.Id);

				performanceLevel.State = Convert.ToByte(EnumState.excluido);
				performanceLevel.UpdateDate = DateTime.Now;

				GestaoAvaliacaoContext.Entry(performanceLevel).State = System.Data.Entity.EntityState.Modified;
				GestaoAvaliacaoContext.SaveChanges();
			}
		}

		#endregion
	}
}
