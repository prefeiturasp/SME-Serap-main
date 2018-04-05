using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Entities.Enumerator;
using GestaoAvaliacao.IRepository;
using GestaoAvaliacao.Repository.Context;
using GestaoAvaliacao.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GestaoAvaliacao.Repository
{
    public class ItemSkillRepository : IItemSkillRepository
    {
        #region CRUD

        public ItemSkill Save(ItemSkill entity)
        {
            using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                DateTime dateNow = DateTime.Now;

                entity.CreateDate = dateNow;
                entity.UpdateDate = dateNow;
                entity.State = Convert.ToByte(EnumState.ativo);

                GestaoAvaliacaoContext.ItemSkill.Add(entity);
                GestaoAvaliacaoContext.SaveChanges();

                return entity;
            }
        }

        public void Update(ItemSkill entity)
        {
            using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                ItemSkill _entity = GestaoAvaliacaoContext.ItemSkill.Include("Item").Include("Skill").FirstOrDefault(a => a.Id == entity.Id);

                if (entity.Item != new Item())
                    _entity.Item = entity.Item;

                _entity.OriginalSkill = entity.OriginalSkill;

                if (entity.Skill != new Skill())
                    _entity.Skill = entity.Skill;

                _entity.UpdateDate = DateTime.Now;

                GestaoAvaliacaoContext.Entry(_entity).State = System.Data.Entity.EntityState.Modified;
                GestaoAvaliacaoContext.SaveChanges();
            }
        }

        public ItemSkill Get(long id)
        {
            var transactionOptions = new System.Transactions.TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted
            };

            using (new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required, transactionOptions))
            {
                using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
                {
                    return GestaoAvaliacaoContext.ItemSkill.FirstOrDefault(a => a.Id == id);
                }
            }
        }

        public IEnumerable<ItemSkill> Load(ref Pager pager)
        {
            var transactionOptions = new System.Transactions.TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted
            };

            using (new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required, transactionOptions))
            {
                using (GestaoAvaliacaoContext gestaoAvaliacaoContext = new GestaoAvaliacaoContext())
                {
                    return pager.Paginate(gestaoAvaliacaoContext.ItemSkill.AsNoTracking().Where(x => x.State == (Byte)EnumState.ativo).OrderBy(x => x.Id));
                }
            }
        }

        public void Delete(long id)
        {
            using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                ItemSkill _entity = GestaoAvaliacaoContext.ItemSkill.FirstOrDefault(a => a.Id == id);

                _entity.State = Convert.ToByte(EnumState.excluido);
                _entity.UpdateDate = DateTime.Now;

                GestaoAvaliacaoContext.Entry(_entity).State = System.Data.Entity.EntityState.Modified;
                GestaoAvaliacaoContext.SaveChanges();
            }
        }

        #endregion

        #region Custom Methods

        public IEnumerable<ItemSkill> GetSkillByItemIds(params long[] idItem)
        {
            var transactionOptions = new System.Transactions.TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted
            };

            using (new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required, transactionOptions))
            {
                using (GestaoAvaliacaoContext ctx = new GestaoAvaliacaoContext())
                {
                    var query =
                        ctx.ItemSkill.AsNoTracking().Include("Item").Include("Skill")
                        .Where(s => s.State == (byte)EnumState.ativo && idItem.Contains(s.Item.Id) && s.Skill.State == (byte)EnumState.ativo);

                    return query.ToList();
                }
            }
        }

        public IEnumerable<ItemSkill> GetSkillsByItemIds(long idItem)
        {
            var transactionOptions = new System.Transactions.TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted
            };

            using (new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required, transactionOptions))
            {
                using (GestaoAvaliacaoContext ctx = new GestaoAvaliacaoContext())
                {
                    var query =
                        ctx.ItemSkill.AsNoTracking().Include("Item").Include("Skill")
                        .Where(s => s.State == (byte)EnumState.ativo && s.Item.Id.Equals(idItem) && s.Skill.State == (byte)EnumState.ativo);

                    return query.ToList();
                }
            }
        }

        public ItemSkill GetSkillByItemId(long idItem)
        {
            var transactionOptions = new System.Transactions.TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted
            };

            using (new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required, transactionOptions))
            {
                using (GestaoAvaliacaoContext ctx = new GestaoAvaliacaoContext())
                {
                    return
                        ctx.ItemSkill.AsNoTracking().Include("Item").Include("Skill")
                        .FirstOrDefault(s => s.State == (byte)EnumState.ativo && s.Item.Id.Equals(idItem) && s.Skill.State == (byte)EnumState.ativo);
                }
            }
        }

        #endregion
    }
}
