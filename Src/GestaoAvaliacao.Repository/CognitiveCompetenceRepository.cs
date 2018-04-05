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
    public class CognitiveCompetenceRepository : ConnectionReadOnly, ICognitiveCompetenceRepository
	{
		#region ReadOnly

		public CognitiveCompetence Get(long id)
		{
            var sql = @"SELECT Id, Description " +
                       "FROM CognitiveCompetence " +
                       "WHERE Id = @id ";

			using (IDbConnection cn = Connection)
			{
				cn.Open();

				var cognitiveCompetence = cn.Query<CognitiveCompetence>(sql, new { id = id }).FirstOrDefault();

				return cognitiveCompetence;
			}
		}

		public IEnumerable<CognitiveCompetence> Load(ref Pager pager, Guid EntityId)
		{

            var sql = @"WITH NumberedCognitiveCompetence AS " +
                        "( " +
                           "SELECT Id, Description, " +
                           "ROW_NUMBER() OVER (ORDER BY Description) AS RowNumber " +
                           "FROM CognitiveCompetence " +
                           "WHERE State  = @state " +
                           "AND EntityId = @entityid " +
                        ") " +
                       "SELECT Id, Description " +
                       "FROM NumberedCognitiveCompetence " +
                       "WHERE RowNumber > ( @pageSize * @page ) " +
                       "AND RowNumber <= ( ( @page + 1 ) * @pageSize ) " +
                       "ORDER BY RowNumber";

            var countSql = @"SELECT COUNT(id) " +
                            "FROM CognitiveCompetence " +
                            "WHERE State = @state " +
                            "AND EntityId = @entityid";

			using (IDbConnection cn = Connection)
			{
				cn.Open();

				var CognitiveCompetence = cn.Query<CognitiveCompetence>(sql, new { state = (Byte)EnumState.ativo, entityid = EntityId, pageSize = pager.PageSize, page = pager.CurrentPage });
				var count = (int)cn.ExecuteScalar(countSql, new { state = (Byte)EnumState.ativo, entityid = EntityId });

				pager.SetTotalPages((int)Math.Ceiling(CognitiveCompetence.Count() / (double)pager.PageSize));
				pager.SetTotalItens(count);

				return CognitiveCompetence;
			}

		}

		public IEnumerable<CognitiveCompetence> Search(string search, ref Pager pager, Guid EntityId)
		{
            var sql = @"WITH NumberedCognitiveCompetence AS " +
                            "( " +
                               "SELECT Id, Description, " +
                               "ROW_NUMBER() OVER (ORDER BY Description) AS RowNumber " +
                               "FROM CognitiveCompetence " +
                               "WHERE State  = @state " +
                               "AND EntityId = @entityid " +
                               "AND (@search IS NULL OR Description Like '%' + @search + '%') " +
                            ") " +
                           "SELECT Id, Description " +
                           "FROM NumberedCognitiveCompetence " +
                           "WHERE RowNumber > ( @pageSize * @page ) " +
                           "AND RowNumber <= ( ( @page + 1 ) * @pageSize ) " +
                           "ORDER BY RowNumber";

            var countSql = @"SELECT COUNT(id) " +
                            "FROM CognitiveCompetence " +
                            "WHERE State = @state " +
                            "AND (@search IS NULL OR Description Like '%' + @search + '%') " +
                            "AND EntityId = @entityid";

			using (IDbConnection cn = Connection)
			{
				cn.Open();				

				var cognitiveCompetence = cn.Query<CognitiveCompetence>(sql, new { state = (Byte)EnumState.ativo, entityid = EntityId, pageSize = pager.PageSize, page = pager.CurrentPage, search = search });
				var count = (int)cn.ExecuteScalar(countSql, new { state = (Byte)EnumState.ativo, entityid = EntityId, search = search });

				pager.SetTotalPages((int)Math.Ceiling(cognitiveCompetence.Count() / (double)pager.PageSize));
				pager.SetTotalItens(count);

				return cognitiveCompetence;
			}
		}

		public bool ExistsDescriptionNamed(string description, Guid EntityId)
		{
            var sql = @"SELECT COUNT(Id) " +
                        "FROM CognitiveCompetence " +
                        "WHERE Description COLLATE Latin1_General_CI_AS = @Description " +
                        "AND EntityId = @entityid " +
                        "AND State = @state ";

			using (IDbConnection cn = Connection)
			{
				cn.Open();				

				var count = (int)cn.ExecuteScalar(sql, new { Description = description, entityid = EntityId, state = (Byte)EnumState.ativo });

				return count > 0;
			}
		}

		public bool ExistsDescriptionNamedAlter(string Description, int id, Guid EntityId)
		{
            var sql = @"SELECT COUNT(Id) " +
                       "FROM CognitiveCompetence " +
                       "WHERE Description COLLATE Latin1_General_CI_AS = @Description " +
                       "AND EntityId = @entityid " +
                       "AND State = @state " +
                       "AND Id != @id ";

			using (IDbConnection cn = Connection)
			{
				cn.Open();

				var count = (int)cn.ExecuteScalar(sql, new { Description = Description, entityid = EntityId, state = (Byte)EnumState.ativo, id = id });
				return count > 0;
			}
		}

		public IEnumerable<CognitiveCompetence> FindAll(Guid EntityId)
		{
            var sql = @"SELECT Id, Description " +
                       "FROM CognitiveCompetence " +
                       "WHERE State = @state " +
                       "AND EntityId = @entityid";

			using (IDbConnection cn = Connection)
			{
				cn.Open();

				var cognitiveCompetence = cn.Query<CognitiveCompetence>(sql, new { state = (Byte)EnumState.ativo, entityid = EntityId });

				return cognitiveCompetence;
			}
		}

		#endregion

		#region CRUD

		public void Delete(long id)
		{
			using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
			{
				CognitiveCompetence cognitiveCompetence = GestaoAvaliacaoContext.CognitiveCompetence.FirstOrDefault(a => a.Id == id);

				cognitiveCompetence.State = Convert.ToByte(EnumState.excluido);
				cognitiveCompetence.UpdateDate = DateTime.Now;

				GestaoAvaliacaoContext.Entry(cognitiveCompetence).State = System.Data.Entity.EntityState.Modified;
				GestaoAvaliacaoContext.SaveChanges();
			}
		}
		public CognitiveCompetence Save(CognitiveCompetence entity)
		{
			using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
			{
				DateTime dateNow = DateTime.Now;

				entity.CreateDate = dateNow;
				entity.UpdateDate = dateNow;
				entity.State = Convert.ToByte(EnumState.ativo);

				GestaoAvaliacaoContext.CognitiveCompetence.Add(entity);
				GestaoAvaliacaoContext.SaveChanges();

				return entity;
			}
		}

		public void Update(CognitiveCompetence entity)
		{
			using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
			{
				CognitiveCompetence cognitiveCompetence = GestaoAvaliacaoContext.CognitiveCompetence.FirstOrDefault(a => a.Id == entity.Id);

				if (!string.IsNullOrEmpty(entity.Description))
					cognitiveCompetence.Description = entity.Description;

				cognitiveCompetence.UpdateDate = DateTime.Now;

				GestaoAvaliacaoContext.Entry(cognitiveCompetence).State = System.Data.Entity.EntityState.Modified;
				GestaoAvaliacaoContext.SaveChanges();
			}
		}
		#endregion
	}
}
