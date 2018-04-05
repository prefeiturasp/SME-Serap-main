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
    public class ItemCurriculumGradeRepository : IItemCurriculumGradeRepository
    {
        #region CRUD

        public ItemCurriculumGrade Save(ItemCurriculumGrade entity)
        {
            using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                DateTime dateNow = DateTime.Now;

                entity.CreateDate = dateNow;
                entity.UpdateDate = dateNow;
                entity.State = Convert.ToByte(EnumState.ativo);

                GestaoAvaliacaoContext.SaveChanges();

                return entity;
            }
        }

        public void Update(ItemCurriculumGrade entity)
        {
            using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                ItemCurriculumGrade _entity = GestaoAvaliacaoContext.ItemCurriculumGrade.Include("Item").FirstOrDefault(a => a.Id == entity.Id);

                if (entity.Item != new Item())
                    _entity.Item = entity.Item;

                _entity.UpdateDate = DateTime.Now;

                GestaoAvaliacaoContext.Entry(_entity).State = System.Data.Entity.EntityState.Modified;
                GestaoAvaliacaoContext.SaveChanges();
            }
        }

        public ItemCurriculumGrade Get(long id)
        {
            var transactionOptions = new System.Transactions.TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted
            };

            using (new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required, transactionOptions))
            {
                using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
                {
                    return GestaoAvaliacaoContext.ItemCurriculumGrade.FirstOrDefault(a => a.Id == id);
                }
            }
        }

        public IEnumerable<ItemCurriculumGrade> Load(ref Pager pager)
        {
            var transactionOptions = new System.Transactions.TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted
            };

            using (new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required, transactionOptions))
            {
                using (GestaoAvaliacaoContext gestaoAvaliacaoContext = new GestaoAvaliacaoContext())
                {
                    return pager.Paginate(gestaoAvaliacaoContext.ItemCurriculumGrade.AsNoTracking().Where(x => x.State == (Byte)EnumState.ativo).OrderBy(x => x.Id));
                }
            }
        }

        public void Delete(long id)
        {
            using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                ItemCurriculumGrade _entity = GestaoAvaliacaoContext.ItemCurriculumGrade.FirstOrDefault(a => a.Id == id);

                _entity.State = Convert.ToByte(EnumState.excluido);
                _entity.UpdateDate = DateTime.Now;

                GestaoAvaliacaoContext.Entry(_entity).State = System.Data.Entity.EntityState.Modified;
                GestaoAvaliacaoContext.SaveChanges();
            }
        }

        #endregion

        #region Custom Methods
        public bool ExistsItemCurriculumGrade(int typeCurriculumGradeId, long evaluationMatrixId)
        {
            var transactionOptions = new System.Transactions.TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted
            };

            using (new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required, transactionOptions))
            {
                using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
                {
                    return GestaoAvaliacaoContext.ItemCurriculumGrade.AsNoTracking().FirstOrDefault(
                        a => a.TypeCurriculumGradeId == typeCurriculumGradeId && a.Item.EvaluationMatrix.Id == evaluationMatrixId && a.State == (Byte)EnumState.ativo && a.Item.State == (Byte)EnumState.ativo && a.Item.EvaluationMatrix.State == (Byte)EnumState.ativo) != null;
                }
            }
        }
        #endregion
    }
}
