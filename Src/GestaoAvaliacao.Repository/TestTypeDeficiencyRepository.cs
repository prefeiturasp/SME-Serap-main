using GestaoAvaliacao.Entities;
using GestaoAvaliacao.IRepository;
using GestaoAvaliacao.Repository.Context;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Repository
{
    public class TestTypeDeficiencyRepository : ConnectionReadOnly, ITestTypeDeficiencyRepository
    {
        public async Task<IEnumerable<TestTypeDeficiency>> GetAsync(long testTypeId)
        {
            var transactionOptions = new System.Transactions.TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted
            };

            using (new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required, transactionOptions))
            using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
                return await GestaoAvaliacaoContext.TestTypeDeficiencies.Where(a => a.TestType.Id == testTypeId).ToListAsync();
        }
    }
}