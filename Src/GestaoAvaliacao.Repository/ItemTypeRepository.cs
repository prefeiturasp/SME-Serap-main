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
    public class ItemTypeRepository : ConnectionReadOnly, IItemTypeRepository
    {
        #region Read

        public ItemType Get(long id)
        {
            var sql = new StringBuilder("SELECT Id,Description,UniqueAnswer,CreateDate,UpdateDate,State,EntityId,IsDefault,QuantityAlternative,IsVisibleTestType ");
            sql.Append("FROM dbo.ItemType WITH (NOLOCK) ");
            sql.Append("WHERE Id = @id");

            using (IDbConnection cn = Connection)
            {
                cn.Open();
                return cn.Query<ItemType>(sql.ToString(), new { id = id }).FirstOrDefault();
            }
        }

        public IEnumerable<ItemType> FindSimple(Guid EntityId)
        {
            var sql = new StringBuilder("SELECT Id,Description,IsDefault,QuantityAlternative ");
            sql.Append("FROM dbo.ItemType WITH (NOLOCK) ");
            sql.Append("WHERE EntityId = @EntityId AND State = @State ");
            sql.Append("ORDER BY IsDefault DESC, Description ");

            using (IDbConnection cn = Connection)
            {
                cn.Open();
                return cn.Query<ItemType>(sql.ToString(), new { EntityId = EntityId, State = Convert.ToByte(EnumState.ativo) });
            }
        }

        public IEnumerable<ItemType> FindForTestType(Guid EntityId)
        {
            var sql = new StringBuilder("SELECT Id,Description,IsDefault,QuantityAlternative ");
            sql.Append("FROM dbo.ItemType WITH (NOLOCK) ");
            sql.Append("WHERE EntityId = @EntityId AND State = @State AND IsVisibleTestType = 1 ");
            sql.Append("ORDER BY IsDefault DESC, Description ");

            using (IDbConnection cn = Connection)
            {
                cn.Open();
                return cn.Query<ItemType>(sql.ToString(), new { EntityId = EntityId, State = Convert.ToByte(EnumState.ativo) });
            }
        }

        public IEnumerable<ItemType> Search(ref Pager pager, Guid EntityId, string search)
        {
            var sql = new StringBuilder("WITH NumberedItemType AS ");
            sql.Append("( ");
            sql.Append("SELECT Id,Description,UniqueAnswer,CreateDate,UpdateDate,State,EntityId,IsDefault,QuantityAlternative, ");
            sql.Append("ROW_NUMBER() OVER (ORDER BY IsDefault DESC, Description) AS RowNumber ");
            sql.Append("FROM ItemType WITH (NOLOCK) ");
            sql.Append("WHERE EntityId = @EntityId AND State <> @State ");

            if (!string.IsNullOrEmpty(search))
                sql.Append("AND Description LIKE '%' + @search + '%' ");

            sql.Append(") ");

            sql.Append("SELECT Id,Description,UniqueAnswer,CreateDate,UpdateDate,State,EntityId,IsDefault,QuantityAlternative ");
            sql.Append("FROM NumberedItemType ");
            sql.Append("WHERE RowNumber > ( @pageSize * @page ) ");
            sql.Append("AND RowNumber <= ( ( @page + 1 ) * @pageSize ) ");

            sql.Append("SELECT COUNT(id) ");
            sql.Append("FROM ItemType ");
            sql.Append("WHERE State <> @state AND EntityId = @entityid ");

            if (!string.IsNullOrEmpty(search))
                sql.Append("AND Description LIKE '%' + @search + '%' ");


            using (IDbConnection cn = Connection)
            {
                cn.Open();
                var query = cn.QueryMultiple(sql.ToString(), new { EntityId = EntityId, State = Convert.ToByte(EnumState.excluido), search = search, pageSize = pager.PageSize, page = pager.CurrentPage });

                var itemTypes = query.Read<ItemType>();
                var count = query.Read<int>().FirstOrDefault();

                pager.SetTotalItens(count);
                pager.SetTotalPages((int)Math.Ceiling(count / (double)pager.PageSize));

                return itemTypes;
            }
        }

        public bool ExistsDescriptionNamed(string description, long Id, Guid EntityId)
        {
            StringBuilder sql = new StringBuilder("SELECT Count(Id) ");
            sql.Append("FROM dbo.ItemType WITH (NOLOCK) ");
            sql.Append("WHERE State = @state AND EntityId = @EntityId AND Id <> @id AND Description COLLATE Latin1_General_CI_AS = @DefaultModel");

            using (IDbConnection cn = Connection)
            {
                cn.Open();

                return (int)cn.ExecuteScalar(sql.ToString(), new { state = (Byte)EnumState.ativo, EntityId = EntityId, id = Id, DefaultModel = description }) > 0;
            }
        }

        #endregion

        #region Write

        public ItemType Save(ItemType entity)
        {
            using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                DateTime dateNow = DateTime.Now;

                entity.CreateDate = dateNow;
                entity.UpdateDate = dateNow;
                entity.State = Convert.ToByte(EnumState.ativo);

                GestaoAvaliacaoContext.ItemType.Add(entity);
                GestaoAvaliacaoContext.SaveChanges();

                return entity;
            }
        }

        public void Update(ItemType entity)
        {
            using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                ItemType _entity = GestaoAvaliacaoContext.ItemType.FirstOrDefault(a => a.Id == entity.Id);

                if (!string.IsNullOrEmpty(entity.Description))
                    _entity.Description = entity.Description;

                _entity.UniqueAnswer = entity.UniqueAnswer;
                _entity.IsDefault = entity.IsDefault;
                _entity.IsVisibleTestType = entity.IsVisibleTestType;
                _entity.State = entity.State;
                _entity.QuantityAlternative = entity.QuantityAlternative;

                _entity.UpdateDate = DateTime.Now;

                GestaoAvaliacaoContext.Entry(_entity).State = System.Data.Entity.EntityState.Modified;
                GestaoAvaliacaoContext.SaveChanges();
            }
        }

        public void Delete(long id)
        {
            using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                ItemType _entity = GestaoAvaliacaoContext.ItemType.FirstOrDefault(a => a.Id == id);

                _entity.State = Convert.ToByte(EnumState.excluido);
                _entity.UpdateDate = DateTime.Now;

                GestaoAvaliacaoContext.Entry(_entity).State = System.Data.Entity.EntityState.Modified;
                GestaoAvaliacaoContext.SaveChanges();
            }
        }

        public void VerifyDefault(Guid EntityId)
        {
            using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                ItemType _entity = GestaoAvaliacaoContext.ItemType.FirstOrDefault(a => a.EntityId == EntityId && a.IsDefault && a.State == (byte)EnumState.ativo);

                if (_entity != null)
                {
                    _entity.IsDefault = false;
                    _entity.UpdateDate = DateTime.Now;

                    GestaoAvaliacaoContext.Entry(_entity).State = System.Data.Entity.EntityState.Modified;
                    GestaoAvaliacaoContext.SaveChanges();
                }
            }
        }

        public bool ExistsItems(long id)
        {

            var sql = @"SELECT CASE WHEN EXISTS (
							SELECT*
							FROM Item
							WHERE ItemType_Id = @id and State = 1
						)
						THEN CAST(1 AS BIT)
						ELSE CAST(0 AS BIT) END";

            using (IDbConnection cn = Connection)
            {
                cn.Open();

                return cn.ExecuteScalar<bool>(sql, new { id });
            }

        }

        #endregion
    }
}
