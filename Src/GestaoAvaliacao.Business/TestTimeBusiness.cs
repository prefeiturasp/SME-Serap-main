using GestaoAvaliacao.Entities;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.IRepository;
using GestaoAvaliacao.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Business
{
    public class TestTimeBusiness : ITestTimeBusiness
    {
        private readonly ITestTimeRepository testTimeRepository;

        public TestTimeBusiness(ITestTimeRepository testTimeRepository, ITestBusiness testBusiness)
        {
            this.testTimeRepository = testTimeRepository;
        }

        public List<TestTime> GetAll() => testTimeRepository.GetAll();

        public async Task<TestTime> GetByTestIdAsync(long testId) => await testTimeRepository.GetByTestIdAsync(testId);

    }
}