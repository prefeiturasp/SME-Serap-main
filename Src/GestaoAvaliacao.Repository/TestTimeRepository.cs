using Dapper;
using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Entities.Enumerator;
using GestaoAvaliacao.IRepository;
using GestaoAvaliacao.Repository.Context;
using GestaoAvaliacao.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Repository
{
    public class TestTimeRepository : ConnectionReadOnly, ITestTimeRepository
    {
        public List<TestTime> GetAll()
        {
            using (var gestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                return gestaoAvaliacaoContext.TestTime.ToList();
            }
        }

        public async Task<TestTime> GetByTestIdAsync(long testId)
        {
            using (var gestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                return await gestaoAvaliacaoContext.Test.Where(f=>f.Id == testId).Select(s => s.TestTime).FirstOrDefaultAsync();
            }
        }
    }
}
