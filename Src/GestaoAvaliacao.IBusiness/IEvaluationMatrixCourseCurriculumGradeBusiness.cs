using GestaoAvaliacao.Entities;
using System;
using System.Collections.Generic;

namespace GestaoAvaliacao.IBusiness
{
    public interface IEvaluationMatrixCourseCurriculumGradeBusiness
	{
		List<EvaluationMatrixCourseCurriculumGrade> GetCurriculumGradesByMatrix(int evaluationMatrixId);

		void SaveList(List<EvaluationMatrixCourseCurriculumGrade> listEntity, int evaluationMatrixId, int courseId, int typeLevelEducationId, int modalityId, Guid ent_id);

		EvaluationMatrixCourseCurriculumGrade GetByCurriculumGradeId(long curriculumGradeId, int evaluationMatrixId, int courseId);
	}
}
