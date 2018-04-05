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
    public class ItemLevelRepository : ConnectionReadOnly, IItemLevelRepository
	{
		#region ReadyOnly

		public ItemLevel Get(long id)
		{
			using (IDbConnection cn = Connection)
			{
				cn.Open();
				var sql = @"SELECT Id, Description, Value " +
						   "FROM ItemLevel " +
						   "WHERE Id = @id ";

				var itemLevel = cn.Query<ItemLevel>(sql, new { id = id }).FirstOrDefault();

				return itemLevel;
			}
		}

		public IEnumerable<ItemLevel> Load(ref Pager pager, Guid EntityId)
		{
			using (IDbConnection cn = Connection)
			{
				cn.Open();
				var sql = @"WITH NumberedItemLevel AS " +
							"( " +
							   "SELECT Id, Description, Value, " +
							   "ROW_NUMBER() OVER (ORDER BY Value) AS RowNumber " +
							   "FROM ItemLevel " +
							   "WHERE State  = @state " +
							   "AND EntityId = @entityid " +
							") " +
						   "SELECT Id, Description, Value " +
						   "FROM NumberedItemLevel " +
						   "WHERE RowNumber > ( @pageSize * @page ) " +
						   "AND RowNumber <= ( ( @page + 1 ) * @pageSize ) " +
						   "ORDER BY RowNumber";

				var countSql = @"SELECT COUNT(id) " +
								"FROM ItemLevel " +
								"WHERE State = @state " +
								"AND EntityId = @entityid";

				var itemLevel = cn.Query<ItemLevel>(sql, new { state = (Byte)EnumState.ativo, entityid = EntityId, pageSize = pager.PageSize, page = pager.CurrentPage });
				var count = (int)cn.ExecuteScalar(countSql, new { state = (Byte)EnumState.ativo, entityid = EntityId });

				pager.SetTotalPages((int)Math.Ceiling(itemLevel.Count() / (double)pager.PageSize));
				pager.SetTotalItens(count);

				return itemLevel;
			}
		}

		public IEnumerable<ItemLevel> Search(String search, ref Pager pager, Guid EntityId)
		{
			using (IDbConnection cn = Connection)
			{
				cn.Open();
				var sql = @"WITH NumberedItemLevel AS " +
							"( " +
							   "SELECT Id, Description, Value, " +
							   "ROW_NUMBER() OVER (ORDER BY Value) AS RowNumber " +
							   "FROM ItemLevel " +
							   "WHERE State  = @state " +
							   "AND EntityId = @entityid " +
							   "AND (@search IS NULL OR Description Like '%' + @search + '%') " +
							") " +
						   "SELECT Id, Description, Value " +
						   "FROM NumberedItemLevel " +
						   "WHERE RowNumber > ( @pageSize * @page ) " +
						   "AND RowNumber <= ( ( @page + 1 ) * @pageSize ) " +
						   "ORDER BY RowNumber";

				var countSql = @"SELECT COUNT(id) " +
								"FROM ItemLevel " +
								"WHERE State = @state " +
								"AND (@search IS NULL OR Description Like '%' + @search + '%') " +
								"AND EntityId = @entityid";

				var itemLevel = cn.Query<ItemLevel>(sql, new { state = (Byte)EnumState.ativo, entityid = EntityId, pageSize = pager.PageSize, page = pager.CurrentPage, search = search });
				var count = (int)cn.ExecuteScalar(countSql, new { state = (Byte)EnumState.ativo, entityid = EntityId, search = search });

				pager.SetTotalPages((int)Math.Ceiling(itemLevel.Count() / (double)pager.PageSize));
				pager.SetTotalItens(count);

				return itemLevel;
			}
		}

		public bool ExistsValue(int value, Guid ent_id)
		{
			using (IDbConnection cn = Connection)
			{
				cn.Open();
				var sql = @"SELECT COUNT(Id) " +
						   "FROM ItemLevel " +
						   "WHERE UPPER(Value) = UPPER(@value) " +
						   "AND EntityId = @entityid " +
						   "AND State = @state ";

				var count = (int)cn.ExecuteScalar(sql, new { value = value, entityid = ent_id, state = (Byte)EnumState.ativo });

				return count > 0;
			}
		}
		
		public bool ExistsValueAlter(int value, long id, Guid ent_id)
		{
			using (IDbConnection cn = Connection)
			{
				cn.Open();
				var sql = @"SELECT COUNT(Id) " +
						   "FROM ItemLevel " +
						   "WHERE UPPER(Value) = UPPER(@value) " +
						   "AND EntityId = @entityid " +
						   "AND State = @state " +
						   "AND Id != @id ";

				var count = (int)cn.ExecuteScalar(sql, new { value = value, entityid = ent_id, state = (Byte)EnumState.ativo, id = id });

				return count > 0;
			}
		}
		
		public bool ExistsTestType(long id, Guid ent_id)
		{
			using (IDbConnection cn = Connection)
			{
				cn.Open();
				var sql = @"SELECT COUNT(ti.Id) " +
						   "FROM TestTypeItemLevel ti " +
						   "INNER JOIN ItemLevel il ON ti.ItemLevel_Id = il.id " +
						   "WHERE ti.ItemLevel_Id = @id " +
						   "AND il.EntityId = @entityid " +
						   "AND il.State = @state " +
						   "AND ti.State = @state ";

				var count = (int)cn.ExecuteScalar(sql, new { id = id, entityid = ent_id, state = (Byte)EnumState.ativo });

				return count > 0;
			}
		}

		public IEnumerable<ItemLevel> LoadLevels(Guid EntityId)
		{
			using (IDbConnection cn = Connection)
			{
				cn.Open();
				var sql = @"SELECT Id, Description, Value " +
						   "FROM ItemLevel " +
						   "WHERE EntityId = @entity " +
						   "AND State = @state " +
						   "ORDER BY Value ";

				var itemLevel = cn.Query<ItemLevel>(sql, new { entity = EntityId, state = (Byte)EnumState.ativo });

				return itemLevel;
			}
		}

		#endregion

		#region CRUD

		public ItemLevel Save(ItemLevel entity)
		{
			using (GestaoAvaliacaoContext gestaoAvaliacaoContext = new GestaoAvaliacaoContext())
			{
				DateTime dateNow = DateTime.Now;
				entity.CreateDate = dateNow;
				entity.UpdateDate = dateNow;
				entity.State = Convert.ToByte(EnumState.ativo);

				gestaoAvaliacaoContext.ItemLevel.Add(entity);
				gestaoAvaliacaoContext.SaveChanges();
			}
			return entity;
		}

		public void Update(ItemLevel entity)
		{
			using (GestaoAvaliacaoContext gestaoAvaliacaoContext = new GestaoAvaliacaoContext())
			{
				ItemLevel itemLevel = gestaoAvaliacaoContext.ItemLevel.FirstOrDefault(il => il.Id == entity.Id);

				if (!string.IsNullOrEmpty(entity.Description))
					itemLevel.Description = entity.Description;

				if (entity.Value > 0)
					itemLevel.Value = entity.Value;

				itemLevel.UpdateDate = DateTime.Now;

				gestaoAvaliacaoContext.Entry(itemLevel).State = System.Data.Entity.EntityState.Modified;

				gestaoAvaliacaoContext.SaveChanges();
			}
		}

		public void Delete(ItemLevel entity)
		{
			using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
			{
				ItemLevel itemLevel = GestaoAvaliacaoContext.ItemLevel.FirstOrDefault(a => a.Id == entity.Id);

				itemLevel.State = Convert.ToByte(EnumState.excluido);
				itemLevel.UpdateDate = DateTime.Now;

				GestaoAvaliacaoContext.Entry(itemLevel).State = System.Data.Entity.EntityState.Modified;
				GestaoAvaliacaoContext.SaveChanges();
			}
		}

		#endregion

	}
}
