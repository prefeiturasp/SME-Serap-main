using GestaoAvaliacao.Entities.DTO;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.IFileServer;
using GestaoAvaliacao.Util;
using GestaoEscolar.IBusiness;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityFile = GestaoAvaliacao.Entities.File;

namespace GestaoAvaliacao.Business
{
	public class ReportStudentPerformanceBusiness : IReportStudentPerformanceBusiness
	{
		private readonly ICorrectionResultsBusiness _correctionResultsBusiness;
		private readonly IBlockBusiness _blockBusiness;
		private readonly IESC_EscolaBusiness _schoolBusiness;
		private readonly ITestBusiness _testBusiness;
		private readonly IFileBusiness _fileBusiness;
		private readonly IStorage _storage;
		private readonly ITUR_TurmaBusiness _turmaBusiness;

		public ReportStudentPerformanceBusiness(ICorrectionResultsBusiness correctionResultsBusiness,
												IBlockBusiness blockBusiness,
												IESC_EscolaBusiness schoolBusiness,
												ITestBusiness testBusiness,
												IFileBusiness fileBusiness,
												IStorage storage,
												ITUR_TurmaBusiness turmaBusiness)
		{
			_correctionResultsBusiness = correctionResultsBusiness;
			_blockBusiness = blockBusiness;
			_schoolBusiness = schoolBusiness;
			_testBusiness = testBusiness;
			_fileBusiness = fileBusiness;
			_storage = storage;
			_turmaBusiness = turmaBusiness;
		}

		/// <summary>
		/// Busca informações de identificação do alu e relacionadas ao desempenho do aluno na prova
		/// </summary>
		/// <param name="test_id">Id da prova</param>
		/// <param name="alu_id">Id do aluno</param>
		/// <param name="dre_id">Id da DRE</param>
		/// <returns>DTO com todas as informações do aluno(identificação e desempenho na prova)</returns>
		public async Task<StudentInformationAndPerformanceTestResult.Student> GetStudentInformation(long test_id, long alu_id, Guid dre_id)
		{
			var studentInformationAndPerformance = new StudentInformationAndPerformanceTestResult.Student();

			var studentInformationAndAverages = await _correctionResultsBusiness.GetStudentTestInformationByTestAndStudent(test_id, alu_id);

			var avgPercentageSMEAndDREPerItem = await _correctionResultsBusiness.GetAvgPercentageSmeAndDrePerItemByTest(test_id, dre_id);

			var itemInformation = await _blockBusiness.GetItemsWithSkillAndCorrectAlternativeBytest(test_id);

			if (studentInformationAndAverages != null)
			{

				studentInformationAndPerformance.Alu_id = studentInformationAndAverages.Alu_id;
				studentInformationAndPerformance.Alu_nome = studentInformationAndAverages.Alu_nome;
				studentInformationAndPerformance.Hits = studentInformationAndAverages.Hits;
				studentInformationAndPerformance.Avg = studentInformationAndAverages.Avg;
				studentInformationAndPerformance.Mtu_numeroChamada = studentInformationAndAverages.Mtu_numeroChamada;

				if (studentInformationAndAverages.Items.Any() && itemInformation.Any())
				{
					studentInformationAndPerformance.Items = (from item in studentInformationAndAverages.Items
															  from itemInfo in itemInformation
															  from avgPerItem in avgPercentageSMEAndDREPerItem
															  where item.Item_Id == itemInfo.Item_Id && item.Item_Id == avgPerItem.Item_Id
															  select new StudentInformationAndPerformanceTestResult.Item
															  {
																  Item_Id = item.Item_Id,
																  ChosenAlternative = item.Numeration.Replace(")", ""),
																  Correct = item.Correct,
																  CorrectAlternative = itemInfo.CorrectAlternativeNumeration.Replace(")", ""),
																  Order = itemInfo.Order,
																  SkillDescription = itemInfo.SkillDescription,
																  SkillCode = itemInfo.SkillCode,
																  AvgSME = (avgPerItem.AvgSME.HasValue && !double.IsNaN(avgPerItem.AvgSME.Value)) ? avgPerItem.AvgSME : 0,
																  AvgDRE = (avgPerItem.AvgDRE.HasValue && !double.IsNaN(avgPerItem.AvgDRE.Value)) ? avgPerItem.AvgDRE : 0,
																  Revoked = itemInfo.Revoked
															  }).OrderBy(x => x.Order).ToList();
				}
			}

			return studentInformationAndPerformance;
		}

		/// <summary>
		/// Busca as informações das unidades(SME, DRE, escola, turma) em relação ao desempenho na prova 
		/// </summary>
		/// <param name="test_id">Id da prova</param>
		/// <param name="esc_id">Id da escola</param>
		/// <param name="dre_id">Id da DRE</param>
		/// <param name="team_id">Id da turma</param>
		/// <returns>DTO com as informações das unidades(SME, DRE, escola, turma) em relação ao desempenho na prova</returns>
		public async Task<UnitsInformationAndPerformanceTestDTO> GetUnitsInformation(long test_id, int esc_id, Guid dre_id, long team_id)
		{

			var unitsInformationAndPerformanceTestDTO = new UnitsInformationAndPerformanceTestDTO();

			var testAveragesHitsAndPercentages = _correctionResultsBusiness
														.GetTestAveragesHitsAndPercentagesByTest(test_id, esc_id, dre_id, team_id, null);

			var school = _schoolBusiness.GetSchoolAndDRENames(esc_id);

			var team = _turmaBusiness.Get(team_id);

			var test = _testBusiness.GetTestById(test_id);

			if (testAveragesHitsAndPercentages != null && school != null && test != null)
			{
				unitsInformationAndPerformanceTestDTO.Esc_nome = school.esc_nome;
				unitsInformationAndPerformanceTestDTO.Uad_nome = school.uad_nome;
				unitsInformationAndPerformanceTestDTO.TestDescription = test.Description;
				unitsInformationAndPerformanceTestDTO.TestDiscipline = test.Discipline.Description;
				unitsInformationAndPerformanceTestDTO.AvgSME = testAveragesHitsAndPercentages.AvgSME;
				unitsInformationAndPerformanceTestDTO.AvgDRE = testAveragesHitsAndPercentages.AvgDRE;
				unitsInformationAndPerformanceTestDTO.AvgESC = testAveragesHitsAndPercentages.AvgESC;
				unitsInformationAndPerformanceTestDTO.AvgTeam = testAveragesHitsAndPercentages.AvgTeam;
				unitsInformationAndPerformanceTestDTO.AvgHitsSME = testAveragesHitsAndPercentages.AvgHitsSME;
				unitsInformationAndPerformanceTestDTO.AvgHitsDRE = testAveragesHitsAndPercentages.AvgHitsDRE;
				unitsInformationAndPerformanceTestDTO.AvgHitsESC = testAveragesHitsAndPercentages.AvgHitsESC;
				unitsInformationAndPerformanceTestDTO.AvgHitsTeam = testAveragesHitsAndPercentages.AvgHitsTeam;
				unitsInformationAndPerformanceTestDTO.Tur_codigo = team.tur_codigo;
			}

			return unitsInformationAndPerformanceTestDTO;
		}

		/// <summary>
		/// Exporta o relatório para CSV
		/// </summary>
		/// <param name="test_id">Id da prova</param>
		/// <param name="alu_id">Id do aluno</param>
		/// <param name="dre_id">Id da DRE</param>
		/// <param name="esc_id">Id da escola</param>
		/// <param name="team_id">Id da turma</param>
		/// <param name="separator">Separador do CSV</param>
		/// <param name="virtualDirectory">Diretório virtual</param>
		/// <param name="physicalDirectory">Diretório físico</param>
		/// <param name="typeUser">Visão do usuário</param>
		/// <returns>Dados para download do CSV</returns>
		public async Task<EntityFile> ExportReport(int test_id, long alu_id, Guid dre_id, int esc_id, long team_id, string separator, string virtualDirectory, string physicalDirectory, EnumSYS_Visao typeUser)
		{
			EntityFile ret = new EntityFile();

			var studentInformation = await GetStudentInformation(test_id, alu_id, dre_id);
			var unitsInformation = await GetUnitsInformation(test_id, esc_id, dre_id, team_id);

			if (studentInformation != null && unitsInformation != null)
			{
				var body = new StringBuilder();

				var fileName = "Rel_Student_Performance";

				body.AppendLine(string.Concat("ESCOLA:", separator, unitsInformation.Esc_nome, separator, "DRE:", separator, unitsInformation.Uad_nome))
					.AppendLine(string.Concat("ALUNO:", separator, studentInformation.Alu_nome, separator, "Nº:", separator, studentInformation.Mtu_numeroChamada));

				body.AppendLine();

				body.AppendLine(string.Concat("ACERTOS DO ALUNO", separator, studentInformation.Hits, separator, studentInformation.Avg, "%"))
					.AppendLine(string.Concat("MÉDIA DE ACERTOS DA TURMA", separator, unitsInformation.AvgHitsTeam, separator, unitsInformation.AvgTeam, "%"));

				if (typeUser != EnumSYS_Visao.Individual)
				{
					body.AppendLine(string.Concat("MÉDIA DE ACERTOS DA ESCOLA", separator, unitsInformation.AvgHitsESC, separator, unitsInformation.AvgESC, "%"));

					if (typeUser == EnumSYS_Visao.Administracao)
						body.AppendLine(string.Concat("MÉDIA DE ACERTOS DA SME", separator, unitsInformation.AvgHitsSME, separator, unitsInformation.AvgSME, "%"));
				}

				body.AppendLine();

				body.AppendLine(string.Concat("ITEM", separator,
											  "HABILIDADE", separator,
											  "ALTERNATIVA", separator,
											  "GABARITO", separator,
											  typeUser == EnumSYS_Visao.Gestao || typeUser == EnumSYS_Visao.Administracao ? "% DRE" : null, separator,
											  typeUser == EnumSYS_Visao.Administracao ? "% SME" : null));

				if (studentInformation.Items.Any())
					studentInformation.Items.ForEach(x =>
					{
						body.AppendLine(string.Concat(x.Order + 1, separator,
												  x.SkillCode, " - ", x.SkillDescription, separator,
												  x.ChosenAlternative, separator,
												  x.CorrectAlternative, separator,
												  typeUser == EnumSYS_Visao.Gestao || typeUser == EnumSYS_Visao.Administracao ? x.AvgDRE : null, separator,
												  typeUser == EnumSYS_Visao.Administracao ? x.AvgSME : null));
					});

				ret = CreateReportCsvFile(fileName, body.ToString(), virtualDirectory, physicalDirectory);
			}
			else
			{
				ret.Validate.IsValid = false;
				ret.Validate.Type = ValidateType.alert.ToString();
				ret.Validate.Message = "Ainda não existem dados para exportar.";
			}

			return ret;
		}

		/// <summary>
		/// Salva o arquivo e monta o objeto de retorno com os dados para download do CSV
		/// </summary>
		/// <param name="fileName">Nome do arquivo</param>
		/// <param name="body">Conteúdo do CSV</param>
		/// <param name="virtualDirectory">Diretório virtual</param>
		/// <param name="physicalDirectory">Diretório físico</param>
		/// <returns>Dados para download do CSV</returns>
		private EntityFile CreateReportCsvFile(string fileName, string body, string virtualDirectory, string physicalDirectory)
		{
			byte[] buffer = System.Text.Encoding.Default.GetBytes(body);
			string originalName = string.Format("{0}.csv", fileName);
			string name = string.Format("{0}.csv", Guid.NewGuid());
			string contentType = MimeType.CSV.GetDescription();

			var csvFiles = _fileBusiness.GetAllFilesByType(EnumFileType.ExportReportStudentPerformance, DateTime.Now.AddDays(-1));
			if (csvFiles != null && csvFiles.Count() > 0)
			{
				_fileBusiness.DeletePhysicalFiles(csvFiles.ToList(), physicalDirectory);
				_fileBusiness.DeleteFilesByType(EnumFileType.ExportReportTestPerformance, DateTime.Now.AddDays(-1));
			}

			EntityFile ret = _storage.Save(buffer, name, contentType, EnumFileType.ExportReportStudentPerformance.GetDescription(), virtualDirectory, physicalDirectory, out ret);

			if (ret.Validate.IsValid)
			{
				ret.OriginalName = StringHelper.Normalize(originalName);
				ret.OwnerType = (byte)EnumFileType.ExportReportTestPerformance;

				ret = _fileBusiness.Save(ret);
			}

			return ret;
		}
	}
}
