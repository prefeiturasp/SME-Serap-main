using GestaoAvaliacao.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoAvaliacao.IRepository
{
    public interface ITestTaiCurriculumGradeRepository
    {
        TestTaiCurriculumGrade Save(TestTaiCurriculumGrade entity);
        List<TestTaiCurriculumGrade> GetListByTestId(long testId);
        TestTaiCurriculumGrade Update(TestTaiCurriculumGrade entity);
    }
}
