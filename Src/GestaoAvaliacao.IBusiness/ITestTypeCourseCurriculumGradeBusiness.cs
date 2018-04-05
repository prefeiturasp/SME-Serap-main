using GestaoAvaliacao.Entities;
using System;
using System.Collections.Generic;

namespace GestaoAvaliacao.IBusiness
{
    public interface ITestTypeCourseCurriculumGradeBusiness
	{
		List<TestTypeCourseCurriculumGrade> GetCurriculumGradesByTestType(int testTypeId);

		void SaveList(List<TestTypeCourseCurriculumGrade> listEntity, int testTypeId, int courseId, int typeLevelEducationId, int modalityId, Guid ent_id);

		TestTypeCourseCurriculumGrade Delete(long id);

		TestTypeCourseCurriculumGrade GetByCurriculumGradeId(long curriculumGradeId, int testTypeId, int courseId);
	}
}
