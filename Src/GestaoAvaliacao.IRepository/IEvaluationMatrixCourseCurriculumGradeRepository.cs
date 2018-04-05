using GestaoAvaliacao.Entities;
using System.Collections.Generic;

namespace GestaoAvaliacao.IRepository
{
    public interface IEvaluationMatrixCourseCurriculumGradeRepository
    {
        List<EvaluationMatrixCourseCurriculumGrade> GetCurriculumGradesByMatrix(int evaluationMatrixId);

        List<EvaluationMatrixCourseCurriculumGrade> GetCurriculumGradesByMatrixCourse(int evaluationMatrixId, int courseId);

        EvaluationMatrixCourseCurriculumGrade SaveList(EvaluationMatrixCourseCurriculumGrade entity, int typeLevelEducationId, int modalityId);

        void Delete(EvaluationMatrixCourseCurriculumGrade entity);

        EvaluationMatrixCourseCurriculumGrade Get(long id);

        void Update(EvaluationMatrixCourseCurriculumGrade entity);

        EvaluationMatrixCourseCurriculumGrade GetByCurriculumGradeId(long curriculumGradeId, int evaluationMatrixId, int courseId);

    }
}
