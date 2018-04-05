using GestaoAvaliacao.Entities;
using System.Collections.Generic;

namespace GestaoAvaliacao.IBusiness
{
    public interface ITestTypeItemLevelBusiness
    {
        TestTypeItemLevel Save(TestTypeItemLevel entity);
        TestTypeItemLevel Update(long id, TestTypeItemLevel entity);
        TestTypeItemLevel Get(long id);
        IEnumerable<TestTypeItemLevel> Load();
        TestTypeItemLevel LoadByTestType(long testTypeId);
    }
}
