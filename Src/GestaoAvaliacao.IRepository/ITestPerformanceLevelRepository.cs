using GestaoAvaliacao.Entities;
using System.Collections.Generic;

namespace GestaoAvaliacao.IRepository
{
    public interface ITestPerformanceLevelRepository
    {
        List<TestPerformanceLevel> GetPerformanceLevelByTest(long TestId);
        bool ExistsTestPerformanceLevel(long idPerformance);
        void DeleteByTestId(long Id);
    }
}
