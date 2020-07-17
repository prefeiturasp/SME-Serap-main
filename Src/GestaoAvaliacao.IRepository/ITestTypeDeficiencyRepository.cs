using GestaoAvaliacao.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestaoAvaliacao.IRepository
{
    public interface ITestTypeDeficiencyRepository
    {
        Task<IEnumerable<TestTypeDeficiency>> GetAsync(long testTypeId);
    }
}