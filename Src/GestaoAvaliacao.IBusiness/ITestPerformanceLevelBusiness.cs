using GestaoAvaliacao.Entities;
using System.Collections.Generic;

namespace GestaoAvaliacao.IBusiness
{
    public interface ITestPerformanceLevelBusiness
    {
        List<TestPerformanceLevel> GetPerformanceLevelByTest(long Id);

        bool ExistsTestPerformanceLevel(long idPerformance);
    }
}
