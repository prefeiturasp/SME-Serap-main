using GestaoAvaliacao.Entities;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.IRepository;

namespace GestaoAvaliacao.Business
{
    public class TestSubGroupBusiness : ITestSubGroupBusiness
    {
        private readonly ITestSubGroupRepository testSubGroupRepository;

        public TestSubGroupBusiness(ITestSubGroupRepository testSubGroupRepository)
        {
            this.testSubGroupRepository = testSubGroupRepository;
        }

        #region Read

        public TestSubGroup Get(long id)
        {
            return testSubGroupRepository.Get(id);
        }

        #endregion

        #region Write

        public void ChangeOrder(long idOrigem, long idDestino)
        {
            TestSubGroup subOrigem = Get(idOrigem);
            TestSubGroup subDestino = Get(idDestino);

            int ordemOrigem = subOrigem.Order;
            int ordemDestino = subDestino.Order;

            subOrigem.Order = ordemDestino;
            subDestino.Order = ordemOrigem;

            testSubGroupRepository.Update(subOrigem);
            testSubGroupRepository.Update(subDestino);
        }

        #endregion
    }
}
