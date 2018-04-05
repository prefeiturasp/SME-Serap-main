using GestaoAvaliacao.Entities;
using System.Collections.Generic;

namespace GestaoAvaliacao.IRepository
{
    public interface ITestTypeItemLevelRepository
    {
        TestTypeItemLevel Save(TestTypeItemLevel entity);
        void Update(TestTypeItemLevel entity);
        TestTypeItemLevel Get(long id);
        IEnumerable<TestTypeItemLevel> Load();
        TestTypeItemLevel LoadByTestType(long testTypeId);
        void Delete(TestTypeItemLevel entity);
    }
}
