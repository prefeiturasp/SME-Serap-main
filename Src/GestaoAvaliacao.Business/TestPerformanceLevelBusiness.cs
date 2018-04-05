using GestaoAvaliacao.Entities;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.IRepository;
using System.Collections.Generic;

namespace GestaoAvaliacao.Business
{
    public class TestPerformanceLevelBusiness : ITestPerformanceLevelBusiness
    {
        private readonly ITestPerformanceLevelRepository testPerformanceLevelRepository;

        public TestPerformanceLevelBusiness(ITestPerformanceLevelRepository testPerformanceLevelRepository)
        {
            this.testPerformanceLevelRepository = testPerformanceLevelRepository;
        }

        #region Read

        public List<TestPerformanceLevel> GetPerformanceLevelByTest(long TestId)
        {
            return testPerformanceLevelRepository.GetPerformanceLevelByTest(TestId);
        }

        public bool ExistsTestPerformanceLevel(long idPerformance)
        {
            return testPerformanceLevelRepository.ExistsTestPerformanceLevel(idPerformance);
        }

        #endregion
    }
}
