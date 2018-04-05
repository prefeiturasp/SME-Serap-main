using GestaoAvaliacao.Entities;
using System.Collections.Generic;

namespace GestaoAvaliacao.IRepository
{
    public interface ITestTypeCourseCurriculumGradeRepository
    {
        List<TestTypeCourseCurriculumGrade> GetCurriculumGradesByTestType(int testTypeId);

        List<TestTypeCourseCurriculumGrade> GetCurriculumGradesByTestTypeCourse(int testTypeId, int courseId);

        TestTypeCourseCurriculumGrade SaveList(TestTypeCourseCurriculumGrade entity, int typeLevelEducationId, int modalityId);

        void Delete(TestTypeCourseCurriculumGrade entity);

        TestTypeCourseCurriculumGrade Get(long id);

        void Update(TestTypeCourseCurriculumGrade entity);

        TestTypeCourseCurriculumGrade GetByCurriculumGradeId(long curriculumGradeId, int testTypeId, int courseId);
    }
}
