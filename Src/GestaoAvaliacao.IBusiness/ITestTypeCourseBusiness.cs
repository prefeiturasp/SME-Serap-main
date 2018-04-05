using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Util;
using System.Collections.Generic;

namespace GestaoAvaliacao.IBusiness
{
    public interface ITestTypeCourseBusiness
    {
        TestTypeCourse Save(TestTypeCourse entity, int TypeLevelEducationId, int ModalityId);
        TestTypeCourse Update(long id, TestTypeCourse entity);
        TestTypeCourse Get(long id);
        TestTypeCourse Delete(long id, long testTypeId);
        IEnumerable<TestTypeCourse> Load(ref Pager pager);
        IEnumerable<TestTypeCourse> Search(int testTypeId, ref Pager pager);
    }
}
