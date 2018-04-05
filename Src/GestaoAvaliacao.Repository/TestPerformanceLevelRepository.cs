using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Entities.Enumerator;
using GestaoAvaliacao.IRepository;
using GestaoAvaliacao.Repository.Context;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GestaoAvaliacao.Repository
{
    public class TestPerformanceLevelRepository : ITestPerformanceLevelRepository
    {
        public List<TestPerformanceLevel> GetPerformanceLevelByTest(long TestId)
        {
            var transactionOptions = new System.Transactions.TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted
            };

            using (new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required, transactionOptions))
            {
                using (GestaoAvaliacaoContext ctx = new GestaoAvaliacaoContext())
                {
                    var query = ctx.TestPerformanceLevel.AsNoTracking()
                        .Where(i => i.Test.Id == TestId && i.State == (Byte)EnumState.ativo).ToList();

                    return query;
                }
            }
        }

        public bool ExistsTestPerformanceLevel(long idPerformance)
        {
            var transactionOptions = new System.Transactions.TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted
            };

            using (new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required, transactionOptions))
            {
                using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
                {
                    return GestaoAvaliacaoContext.TestPerformanceLevel.Include("Test").FirstOrDefault(
                        a => a.PerformanceLevel.Id == idPerformance && a.Test.State != (Byte)EnumState.excluido && a.State != (Byte)EnumState.excluido) != null;
                }
            }
        }

        public void DeleteByTestId(long Id)
        {

            using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                List<TestPerformanceLevel> lstTestPerformanceLevel = GestaoAvaliacaoContext.TestPerformanceLevel.Where(a => a.Test.Id == Id).ToList();

                foreach (TestPerformanceLevel performance in lstTestPerformanceLevel)
                {
                    performance.State = Convert.ToByte(EnumState.excluido);
                    performance.UpdateDate = DateTime.Now;

                    GestaoAvaliacaoContext.Entry(performance).State = System.Data.Entity.EntityState.Modified;
                    GestaoAvaliacaoContext.SaveChanges();
                }
            }            
        }

    }
}
