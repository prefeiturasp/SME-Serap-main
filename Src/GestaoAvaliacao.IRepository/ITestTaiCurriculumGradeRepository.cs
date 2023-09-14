using GestaoAvaliacao.Entities;
using System.Collections.Generic;

namespace GestaoAvaliacao.IRepository
{
    public interface ITestTaiCurriculumGradeRepository
    {
        TestTaiCurriculumGrade Save(TestTaiCurriculumGrade entity);
        List<TestTaiCurriculumGrade> GetListByTestId(long testId);
        TestTaiCurriculumGrade Update(TestTaiCurriculumGrade entity);
        void DeleteByTestId(long testId);
    }
}
