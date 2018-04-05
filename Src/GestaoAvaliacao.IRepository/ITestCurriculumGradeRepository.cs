using GestaoEscolar.Entities;
using System.Collections.Generic;

namespace GestaoAvaliacao.IRepository
{
	public interface ITestCurriculumGradeRepository
	{
		bool ExistsTestCurriculumGrade(int typeCurriculumGradeId, long testTypeId);

		bool ExistsTestCourse(IEnumerable<long> TypeLevelEducationList, long testTypeId);

		IEnumerable<ACA_CurriculoPeriodo> GetSimple(long test_id, int esc_id);
		ACA_CurriculoPeriodo GetTestCurriculumGradeCrpDescricao(long test_id);
		IEnumerable<ACA_TipoCurriculoPeriodo> GetDistinctCurricumGradeByTestSubGroup_Id(long TestSubGroup_Id);
		IEnumerable<ACA_TipoCurriculoPeriodo> GetCurricumGradeByTest_Id(long Test_Id);

	}
}
