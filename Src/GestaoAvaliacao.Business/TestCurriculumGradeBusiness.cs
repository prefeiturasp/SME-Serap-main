using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.IRepository;
using GestaoEscolar.Entities;
using System.Collections.Generic;

namespace GestaoAvaliacao.Business
{
	public class TestCurriculumGradeBusiness : ITestCurriculumGradeBusiness
	{
		private readonly ITestCurriculumGradeRepository testCurriculumGradeReposity;

		#region Read

		public TestCurriculumGradeBusiness(ITestCurriculumGradeRepository testCurriculumGradeReposity)
		{
			this.testCurriculumGradeReposity = testCurriculumGradeReposity;
		}

		public bool ExistsTestCurriculumGrade(int typeCurriculumGradeId, long testTypeId)
		{
			return testCurriculumGradeReposity.ExistsTestCurriculumGrade(typeCurriculumGradeId, testTypeId);
		}

		public bool ExistsTestCourse(IEnumerable<long> TypeLevelEducationList, long testTypeId)
		{
			return testCurriculumGradeReposity.ExistsTestCourse(TypeLevelEducationList, testTypeId);
		}

		public IEnumerable<GestaoEscolar.Entities.ACA_CurriculoPeriodo> GetSimple(long test_id, int esc_id)
		{
			return testCurriculumGradeReposity.GetSimple(test_id, esc_id);
		}

		public ACA_CurriculoPeriodo GetTestCurriculumGradeCrpDescricao(long test_id)
		{
			return testCurriculumGradeReposity.GetTestCurriculumGradeCrpDescricao(test_id);
		}
		public IEnumerable<ACA_TipoCurriculoPeriodo> GetDistinctCurricumGradeByTestSubGroup_Id(long TestSubGroup_Id)
		{
			return testCurriculumGradeReposity.GetDistinctCurricumGradeByTestSubGroup_Id(TestSubGroup_Id);
		}
		public IEnumerable<ACA_TipoCurriculoPeriodo> GetCurricumGradeByTest_Id(long Test_Id)
		{
			return testCurriculumGradeReposity.GetCurricumGradeByTest_Id(Test_Id);
		}

		#endregion

	}
}
