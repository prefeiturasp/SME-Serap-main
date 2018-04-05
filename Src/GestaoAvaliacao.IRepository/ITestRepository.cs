using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Entities.DTO;
using GestaoAvaliacao.Util;
using System;
using System.Collections.Generic;

namespace GestaoAvaliacao.IRepository
{
	public interface ITestRepository
	{
		Test Save(Test entity, Guid usu_id);
		Test Update(Test entity);
		Test Update(long Id, Test entity);
		Test GetTestBy_Id(long Id);

		Test GetTestById(long Id);
		List<Test> GetByTestType(long testTypeId);
		Test UpdateSituation(long Id, EnumTestSituation Situation);
		IEnumerable<TestResult> _SearchTestsUser(TestFilter filter, ref Pager pager);
		IEnumerable<TestResult> _SearchTests(TestFilter filter, ref Pager pager);
		void Delete(long id);
		Test GetObject(long Id);
		AdherenceTest GetObjectToAdherence(long Id);
		void SwitchAllAdhrered(Test test);
		List<AnswerSheetBatchItems> GetTestAnswers(long Id);
		KeyValuePair<long, long> GetTestItem(long Id, int ItemOrder, int AlternativeOrder);
		IEnumerable<AnswerSheetStudentInformation> GetTeamStudents(int SchoolId, long SectionId, long StudentId, long test_id, bool allAdhered);
		IEnumerable<TestItemLevel> GetTestItems(long Id);
		Test GetObjectWithTestType(long Id);
		IEnumerable<AnswerSheetBatch> GetTestAutomaticCorrectionSituation(long testId, long schoolId);
		IEnumerable<Test> GetByTypeCurriculumGrade(int typeCurriculumGrade);
		IEnumerable<Test> TestByUser(TestFilter filter);
		IEnumerable<Test> GetInCorrection();
		IEnumerable<CorrectionStudentGrid> GetByTestSection(long test_id, long tur_id);
		Test GetObjectWithTestTypeItemType(long Id);
		void UpdateTestFeedback(long Id, bool publicFeedback);
		IEnumerable<TestResult> GetTestByDate(TestFilter filter);
		IEnumerable<TestResult> GetTestByDateWithGroup(TestFilter filter);
		IEnumerable<Test> GetTestFinishedCorrection(bool allTests);
		void UpdateTestProcessedCorrection(long Id, bool processedCorrection);
		void UpdateTestVisible(long Id, bool visible);
		IEnumerable<DisciplineItem> GetDisciplineItemByTestId(long test_id);
		Test SelectOrderTestUp(long order);
		Test SelectOrderTestDown(long order);
		IEnumerable<TestResult> GetTestsBySubGroup(long id);
        List<ElectronicTestDTO> SearchEletronicTests();
        List<ElectronicTestDTO> SearchEletronicTestsByPesId(Guid pes_id);
        Test SearchInfoTest(long test_id);
        bool ExistsAdherenceByAluIdTestId(long alu_id, long test_id);
        IEnumerable<TestResult> GetTestsBySubGroupTcpId(long Id, long tcp_id);
    }
}
