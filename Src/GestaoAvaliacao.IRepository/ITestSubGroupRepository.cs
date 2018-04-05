using GestaoAvaliacao.Entities;

namespace GestaoAvaliacao.IRepository
{
    public interface ITestSubGroupRepository
    {
        TestSubGroup Get(long id);
        TestSubGroup Update(TestSubGroup entity);
    }
}
