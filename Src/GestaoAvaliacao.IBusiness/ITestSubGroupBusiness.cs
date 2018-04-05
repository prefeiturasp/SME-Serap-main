using GestaoAvaliacao.Entities;

namespace GestaoAvaliacao.IBusiness
{
    public interface ITestSubGroupBusiness
    {
        TestSubGroup Get(long id);
        void ChangeOrder(long idOrigem, long idDestino);
    }
}
