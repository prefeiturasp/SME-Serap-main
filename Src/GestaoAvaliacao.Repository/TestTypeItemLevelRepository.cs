using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Entities.Enumerator;
using GestaoAvaliacao.IRepository;
using GestaoAvaliacao.Repository.Context;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GestaoAvaliacao.Repository
{
    public class TestTypeItemLevelRepository : ITestTypeItemLevelRepository
    {
        #region CRUD

        public TestTypeItemLevel Save(TestTypeItemLevel entity)
        {
            using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                DateTime dateNow = DateTime.Now;

                entity.CreateDate = dateNow;
                entity.UpdateDate = dateNow;
                entity.State = Convert.ToByte(EnumState.ativo);
                entity.ItemLevel = GestaoAvaliacaoContext.ItemLevel.FirstOrDefault(a => a.Id == entity.ItemLevel.Id);
                entity.TestType = GestaoAvaliacaoContext.TestType.FirstOrDefault(a => a.Id == entity.TestType.Id);

                GestaoAvaliacaoContext.TestTypeItemLevel.Add(entity);
                GestaoAvaliacaoContext.SaveChanges();

                return entity;
            }
        }

        public void Update(TestTypeItemLevel entity)
        {
            using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                TestTypeItemLevel testTypeItemLevel = GestaoAvaliacaoContext.TestTypeItemLevel.FirstOrDefault(
                    a => a.Id == entity.Id);

                if (entity.Value != null)
                    testTypeItemLevel.Value = entity.Value;

                testTypeItemLevel.UpdateDate = DateTime.Now;

                GestaoAvaliacaoContext.Entry(testTypeItemLevel).State = System.Data.Entity.EntityState.Modified;
                GestaoAvaliacaoContext.SaveChanges();
            }
        }

        public void Delete(TestTypeItemLevel entity)
        {
            using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                TestTypeItemLevel testTypeItemLevel = GestaoAvaliacaoContext.TestTypeItemLevel.FirstOrDefault(a => a.Id == entity.Id);

                testTypeItemLevel.State = Convert.ToByte(EnumState.excluido);
                testTypeItemLevel.UpdateDate = DateTime.Now;

                GestaoAvaliacaoContext.Entry(testTypeItemLevel).State = System.Data.Entity.EntityState.Modified;
                GestaoAvaliacaoContext.SaveChanges();
            }
        }



        public TestTypeItemLevel Get(long id)
        {
            var transactionOptions = new System.Transactions.TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted
            };

            using (new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required, transactionOptions))
            {
                using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
                {
                    return GestaoAvaliacaoContext.TestTypeItemLevel.FirstOrDefault(a => a.Id == id);
                }
            }
        }

        public IEnumerable<TestTypeItemLevel> Load()
        {
            var transactionOptions = new System.Transactions.TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted
            };

            using (new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required, transactionOptions))
            {
                using (GestaoAvaliacaoContext gestaoAvaliacaoContext = new GestaoAvaliacaoContext())
                {
                    return gestaoAvaliacaoContext.TestTypeItemLevel.AsNoTracking().Where(x => x.State == (Byte)EnumState.ativo);
                }
            }
        }

        #endregion

        #region Custom Methods

        public TestTypeItemLevel LoadByTestType(long testTypeId)
        {
            var transactionOptions = new System.Transactions.TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted
            };

            using (new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required, transactionOptions))
            {
                using (GestaoAvaliacaoContext gestaoAvaliacaoContext = new GestaoAvaliacaoContext())
                {
                    return gestaoAvaliacaoContext.TestTypeItemLevel.FirstOrDefault(x => x.State == (Byte)EnumState.ativo && x.TestType.Id == testTypeId && x.TestType.State == (Byte)EnumState.ativo);
                }
            }
        }
        #endregion
    }
}
