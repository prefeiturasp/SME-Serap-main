using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Entities.DTO;
using GestaoAvaliacao.Entities.Enumerator;
using GestaoAvaliacao.Util;
using MSTech.CoreSSO.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;

namespace GestaoAvaliacao.IBusiness
{
	public interface ITestBusiness
	{
		Test Save(Test entity, Guid usu_id, bool isAdmin);
		Test Update(long Id, Test entity, Guid UsuId, bool isAdmin);
		Test Update2(long Id, Test entity, Guid UsuId, bool isAdmin);
		Test GetTestById(long Id);
		List<Test> GetByTestType(long Id);
		Test FinallyTest(long Id, Guid UsuId, bool isAdmin);
		IEnumerable<TestResult> _SearchTests(TestFilter filter, ref Pager pager);
		Test Delete(long Id);
		Test GetObject(long Id);

		Test GetTestBy_Id(long Id);
		AdherenceTest GetObjectToAdherence(long Id);
		void SwitchAllAdhrered(Test test);
		List<AnswerSheetBatchItems> GetTestAnswers(long Id);
		KeyValuePair<long, long> GetTestItem(long Id, int ItemOrder, int AlternativeOrder);
		IEnumerable<AnswerSheetStudentInformation> GetTeamStudents(int SchoolId, long SectionId, long StudentId, long test_id, bool allAdhered);
		Test GetObjectWithTestType(long Id);
		IEnumerable<BlockItem> GetItemsByTest(long test_id, Guid UsuId, ref Pager pager);
		Task<IEnumerable<BlockItem>> GetItemsByTestAsync(long test_id, Guid UsuId, int page, int pageItens);
		IEnumerable<BlockItem> GetPendingRevokeItems(ref Pager pager, string ItemCode, DateTime? StartDate, DateTime? EndDate, EnumSituation? Situation);
		EnumTestSituation TestSituation(Test entity);
		IEnumerable<AnswerSheetBatch> GetTestAutomaticCorrectionSituation(long testId, long schoolId);
		IEnumerable<Test> GetByTypeCurriculumGrade(int typeCurriculumGrade);
		IEnumerable<Test> TestByUser(TestFilter filter);
		Validate CanEdit(long TestId, Guid UsuId, EnumSYS_Visao vision);
		IEnumerable<Test> GetInCorrection();
		IEnumerable<CorrectionStudentGrid> GetByTestSection(long test_id, long tur_id);
		Test GetObjectWithTestTypeItemType(long Id);
		GenerateTestDTO GenerateTest(long Id, bool sheet, bool publicFeedback, bool CDNMathJax, string separator, SYS_Usuario Usuario, SYS_Grupo Grupo, string UrlSite, string VirtualDirectory, string PhysicalDirectory);
		GenerateTestDTO ExportTestDoc(long Id, PDFFilter filter, SYS_Usuario Usuario, SYS_Grupo Grupo);
		string GetTestFeedbackHtml(Test entity);
		IEnumerable<TestResult> GetTestByDate(TestFilter filter);
		IEnumerable<TestResult> GetTestByDateWithGroup(TestFilter filter);
		IEnumerable<Test> GetTestFinishedCorrection(bool allTests);
		void UpdateTestProcessedCorrection(long Id, bool processedCorrection);
		void UpdateTestVisible(long Id, bool visible);
		ReportCorrectionTestResult GetInfoReportCorrection(long Test_id);
		ReportCorrectionTestResult GetInfoReportCorrection(long Test_id, Guid ent_id, Guid uad_id);
		ReportCorrectionTestResult GetInfoReportCorrection(long Test_id, Guid ent_id, Guid uad_id, long esc_id);
		ReportCorrectionTestResult GetInfoReportCorrection(long Test_id, Guid ent_id, Guid uad_id, long esc_id, long tur_id);
		IEnumerable<DisciplineItem> GetDisciplineItemByTestId(long test_id);
		void ChangeOrderTestUp(long Id, long order);
		void ChangeOrderTestDown(long Id, long order);
		IEnumerable<TestResult> GetTestsBySubGroup(long id);

		TestShowVideoAudioFilesDto GetTestShowVideoAudioFiles(long testId);
		Task<List<ElectronicTestDTO>> SearchEletronicTests();
		Task<Test> SearchInfoTestAsync(long test_id);
		Task<List<ElectronicTestDTO>> SearchEletronicTestsByPesId(Guid pes_id);
		bool ExistsAdherenceByAluIdTestId(long alu_id, long test_id);
		void ChangeOrder(long idOrigem, long idDestino);
		IEnumerable<TestResult> GetTestsBySubGroupTcpId(long id, long tcp_id);
		Task<ElectronicTestDTO> GetElectronicTestByPesIdAndTestId(Guid pes_id, long testId);

		Task<List<ElectronicTestDTO>> GetTestsByPesId(Guid pes_id);

		void TestTaiCurriculumGradeSave(List<TestTaiCurriculumGrade> entity);
		List<TestTaiCurriculumGrade> GetListTestTaiCurriculumGrade(long testId);
        bool ExistsAdherenceByTestId(long test_id);
        void ImportarCvsBlocos(HttpPostedFileBase arquivo, int testId, Guid usuId, EnumSYS_Visao vision);
    }
}
