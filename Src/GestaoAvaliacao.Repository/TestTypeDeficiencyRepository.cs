using GestaoAvaliacao.Entities;
using GestaoAvaliacao.IRepository;
using GestaoAvaliacao.Repository.Context;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using GestaoAvaliacao.Entities.Enumerator;

namespace GestaoAvaliacao.Repository
{
    public class TestTypeDeficiencyRepository : ConnectionReadOnly, ITestTypeDeficiencyRepository
    {
        public async Task<IEnumerable<TestTypeDeficiency>> GetAsync(long testTypeId)
        {
            using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
                return await GestaoAvaliacaoContext.TestTypeDeficiencies.Where(a => a.TestType.Id == testTypeId).ToListAsync();
        }

        public IEnumerable<TestTypeDeficiency> Get(long testTypeId)
        {
            using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
                return GestaoAvaliacaoContext.TestTypeDeficiencies.Where(a => a.TestType.Id == testTypeId).ToList();
        }

        public IEnumerable<Guid> GetDeficienciesIds(long testTypeId)
        {
            using (var gestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                return gestaoAvaliacaoContext.TestTypeDeficiencies
                    .Where(a => a.TestType_Id == testTypeId && a.State != (byte)EnumState.excluido)
                    .Select(a => a.DeficiencyId)
                    .ToList();
            }
        }
    }
}