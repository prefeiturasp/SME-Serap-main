using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Util;
using System.Collections.Generic;

namespace GestaoAvaliacao.IRepository
{
    public interface ITestTypeCourseRepository
    {
        TestTypeCourse Save(TestTypeCourse entity);
        void Update(TestTypeCourse entity);
        TestTypeCourse Get(long id);
        IEnumerable<TestTypeCourse> Load(ref Pager pager);
        void Delete(TestTypeCourse entity);
        IEnumerable<TestTypeCourse> Search(int testTypeId, ref Pager pager);
        bool ExistCourse(int courseId, int modalityId, long testTypeId);
    }
}
