using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Entities.DTO;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.IFileServer;
using GestaoAvaliacao.IRepository;
using GestaoAvaliacao.MongoEntities;
using GestaoAvaliacao.MongoEntities.DTO;
using GestaoAvaliacao.MongoEntities.Projections;
using GestaoAvaliacao.Util;
using GestaoEscolar.IBusiness;
using MSTech.CoreSSO.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityFile = GestaoAvaliacao.Entities.File;

namespace GestaoAvaliacao.Business
{
	public class CorrectionResultsBusiness : ICorrectionResultsBusiness
	{
		private readonly ICorrectionResultsRepository correctionResultsRepository;
		private readonly ITestTemplateRepository testTemplateRepository;
		private readonly ISectionTestStatsBusiness sectionTestStatsBusiness;
		private readonly IStudentCorrectionBusiness studentCorrectionBusiness;
		private readonly IBlockBusiness blockBusiness;
		private readonly IStudentTestAbsenceReasonBusiness studentTestAbsenceReasonBusiness;
		private readonly ITestBusiness testBusiness;
		private readonly ITestSectionStatusCorrectionBusiness testSectionStatusCorrectionBusiness;
		private readonly ITUR_TurmaBusiness turmaBusiness;
		private readonly IParameterBusiness parameterBusiness;
		private readonly IFileBusiness fileBusiness;
		private readonly IStorage storage;
		private readonly IRequestRevokeBusiness requestRevokeBusiness;
		private readonly IAdherenceBusiness adherenceBusiness;

		public CorrectionResultsBusiness(ICorrectionResultsRepository correctionResultsRepository, ITestTemplateRepository testTemplateRepository,
			ISectionTestStatsBusiness sectionTestStatsBusiness, IStudentCorrectionBusiness studentCorrectionBusiness,
			IBlockBusiness blockBusiness, IStudentTestAbsenceReasonBusiness studentTestAbsenceReasonBusiness, ITestBusiness testBusiness,
			ITestSectionStatusCorrectionBusiness testSectionStatusCorrectionBusiness, IParameterBusiness parameterBusiness, ITUR_TurmaBusiness turmaBusiness,
			IFileBusiness fileBusiness, IStorage storage, IRequestRevokeBusiness requestRevokeBusiness, IAdherenceBusiness adherenceBusiness)
		{
			this.correctionResultsRepository = correctionResultsRepository;
			this.testTemplateRepository = testTemplateRepository;
			this.sectionTestStatsBusiness = sectionTestStatsBusiness;
			this.studentCorrectionBusiness = studentCorrectionBusiness;
			this.blockBusiness = blockBusiness;
			this.studentTestAbsenceReasonBusiness = studentTestAbsenceReasonBusiness;
			this.testBusiness = testBusiness;
			this.testSectionStatusCorrectionBusiness = testSectionStatusCorrectionBusiness;
			this.parameterBusiness = parameterBusiness;
			this.turmaBusiness = turmaBusiness;
			this.fileBusiness = fileBusiness;
			this.storage = storage;
			this.requestRevokeBusiness = requestRevokeBusiness;
			this.adherenceBusiness = adherenceBusiness;
		}


		//	return dto;
		//}

		/// <summary>
		/// Busca as médias(acertos e porcentagem) da SME, DRE, escola e turma da prova passada(test_id)
		/// </summary>
		/// <param name="test_Id">Id da prova</param>
		/// <param name="esc_Id">Id da escola</param>
		/// <param name="dre_id">Id da DRE</param>
		/// <param name="team_id">Id da turma</param>
		/// <returns>Projection com médias(acertos e porcentagem) da SME, DRE, escola e turma em relação a prova passada</returns>
		public TestAveragesHitsAndPercentagesProjection GetTestAveragesHitsAndPercentagesByTest(long test_id, int? esc_id, Guid? dre_id, long? team_id, long? discipline_id)
		{
			List<long> testid = new List<long>();
			testid.Add(test_id);

			var lstResultSME = correctionResultsRepository
								.GetTestAveragesHitsAndPercentagesByTest(testid, discipline_id);


			var resultSME = new TestAverageTeamDTO(lstResultSME, null, null, null).resultSME;
			var resultDRE = new TestAverageTeamDTO(lstResultSME, dre_id, null, null).resultDRE;
			var resultSchool = new TestAverageTeamDTO(lstResultSME, dre_id, esc_id, null).resultSchool;
			var resultTeam = new TestAverageTeamDTO(lstResultSME, dre_id, esc_id, team_id).resultTeam;



			var average = new TestAveragesHitsAndPercentagesProjection
			{
				Test_Id = test_id,

				AvgSME = resultSME.Media,
				AvgDRE = resultDRE.Media,
				AvgESC = resultSchool.Media,
				AvgTeam = resultTeam.Media,

				AvgHitsSME = resultSME.TotalCorretItems,
				AvgHitsDRE = resultDRE.TotalCorretItems,
				AvgHitsESC = resultSchool.TotalCorretItems,
				AvgHitsTeam = resultTeam.TotalCorretItems,
			};

			var dto = new TestAveragesHitsAndPercentagesProjection
			{
				Test_Id = test_id,
				AvgSME = double.IsNaN(average.AvgSME.HasValue ? average.AvgSME.Value : 0) ? 0 : average.AvgSME,
				AvgDRE = double.IsNaN(average.AvgDRE.HasValue ? average.AvgDRE.Value : 0) ? 0 : average.AvgDRE,
				AvgESC = double.IsNaN(average.AvgESC.HasValue ? average.AvgESC.Value : 0) ? 0 : average.AvgESC,
				AvgTeam = double.IsNaN(average.AvgTeam.HasValue ? average.AvgTeam.Value : 0) ? 0 : average.AvgTeam,
				AvgHitsSME = average.AvgHitsSME,
				AvgHitsDRE = average.AvgHitsDRE,
				AvgHitsESC = average.AvgHitsESC,
				AvgHitsTeam = average.AvgHitsTeam
			};

			return dto;
		}
		/// <summary>
		/// Busca as médias(acertos e porcentagem) da SME, DRE, escola e turma da prova passada(test_id)
		/// </summary>
		/// <param name="test_Id">Id da prova</param>
		/// <param name="esc_Id">Id da escola</param>
		/// <param name="dre_id">Id da DRE</param>
		/// <param name="team_id">Id da turma</param>
		/// <returns>Projection com médias(acertos e porcentagem) da SME, DRE, escola e turma em relação a prova passada</returns>
		public TestAveragesHitsAndPercentagesProjection GetAveragesByTest(long test_id, int? esc_id, Guid? dre_id, long? tcp_id, long? discipline_id, SYS_Usuario user, SYS_Grupo group)
		{
			List<long> ltest_id = new List<long>();
			ltest_id.Add(test_id);

			return GetAverages(ltest_id, esc_id, dre_id, tcp_id, discipline_id, user, group);

		}
		/// <summary>
		/// Busca as médias(acertos e porcentagem) da SME, DRE, escola e turma das provas do TestSubGroup_Id 
		/// </summary>
		/// <param name="test_Id">Id da prova</param>
		/// <param name="esc_Id">Id da escola</param>
		/// <param name="dre_id">Id da DRE</param>
		/// <param name="team_id">Id da turma</param>
		/// <returns>Projection com médias(acertos e porcentagem) da SME, DRE, escola e turma em relação a prova passada</returns>
		public TestAveragesHitsAndPercentagesProjection GetAveragesByTestSubGroup_Id(long subgroup_id, int? esc_id, Guid? dre_id, long? tcp_id, long? discipline_id, SYS_Usuario user, SYS_Grupo group)
		{

			var tests = testBusiness.GetTestsBySubGroupTcpId(subgroup_id, tcp_id.Value);
			List<long> ltest_id = new List<long>();
			foreach (var test in tests)
			{
				ltest_id.Add(test.TestId);
			}

			return GetAverages(ltest_id, esc_id, dre_id, tcp_id, discipline_id, user, group);

		}

		private TestAveragesHitsAndPercentagesProjection GetAverages(List<long> ltest_id, int? esc_id, Guid? dre_id, long? tcp_id, long? discipline_id, SYS_Usuario user, SYS_Grupo group)
		{
			var lstResultSME = correctionResultsRepository
								.GetTestAveragesHitsAndPercentagesByTest(ltest_id, discipline_id);

			long? team_id = null;
			var resultSME = new TestAverageTeamDTO(lstResultSME, dre_id, esc_id, team_id).resultSME;
			var resultDRE = new TestAverageTeamDTO(lstResultSME, dre_id, esc_id, team_id).resultDRE;
			var resultSchool = new TestAverageTeamDTO(lstResultSME, dre_id, esc_id, team_id).resultSchool;
			var teams = new List<TeamsDTO>();
			
			teams.AddRange(adherenceBusiness.GetSectionByTestAndTcpId(ltest_id, dre_id, esc_id, tcp_id, user, group));

			var resultTeams = new TestAverageTeamDTO(lstResultSME, dre_id, esc_id, team_id).resultTeams;

			var rTeams = (from rt in resultTeams
						  join t in teams on new { Tur_id = rt.Tur_id, Test_id = rt.Test_id } equals new { Tur_id = (long?)t.tur_id, Test_id = (long?)t.test_id }
						  select new TestAverageTeamResult
						  {
                              Test_id = t.test_id,
                              Dre_id = rt.Dre_id,
							  Esc_id = rt.Esc_id,
							  Tur_id = rt.Tur_id,
							  TotalItems = rt.TotalItems,
							  TotalCorretItems = rt.TotalCorretItems,
							  Tur_codigo = t.tur_codigo
						  }).Distinct().ToList();

			var average = new TestAveragesHitsAndPercentagesProjection
			{

				AvgSME = resultSME.Media,
				AvgDRE = resultDRE.Media,
				AvgESC = resultSchool.Media,
				AvgTeams = rTeams
			};

			var dto = new TestAveragesHitsAndPercentagesProjection
			{
				AvgSME = double.IsNaN(average.AvgSME.HasValue ? average.AvgSME.Value : 0) ? 0 : average.AvgSME,
				AvgDRE = double.IsNaN(average.AvgDRE.HasValue ? average.AvgDRE.Value : 0) ? 0 : average.AvgDRE,
				AvgESC = double.IsNaN(average.AvgESC.HasValue ? average.AvgESC.Value : 0) ? 0 : average.AvgESC,
				AvgTeam = double.IsNaN(average.AvgTeam.HasValue ? average.AvgTeam.Value : 0) ? 0 : average.AvgTeam,
				AvgTeams = average.AvgTeams
			};

			return dto;

		}

		/// <summary>
		/// Busca as médias(porcentagem) de acertos da SME e DRE em relação a cada item da prova
		/// </summary>
		/// <param name="test_Id">Id da prova</param>
		/// <param name="dre_id">Id da DRE</param>
		/// <returns>Lista de projection com as médias(porcentagem) da SME e DRE em relação a cada item da prova</returns>
		public async Task<List<ItemAvgPercentageSMEAndDREProjection>> GetAvgPercentageSmeAndDrePerItemByTest(long test_Id, Guid dre_id)
		{
			var averagesItem = await correctionResultsRepository
								.GetAvgPercentageSmeAndDrePerItemByTest(test_Id, dre_id);

			return averagesItem;
		}

		/// <summary>
		/// Busca as informações do aluno na prova informada, como nome, número de matricula e desempenho na mesma
		/// </summary>
		/// <param name="test_Id">Id da prova</param>
		/// <param name="alu_id">Id do aluno</param>
		/// <returns>Projection com as informações do aluno em determinada prova</returns>
		public async Task<StudentTestInformationProjection> GetStudentTestInformationByTestAndStudent(long test_Id, long alu_id)
		{
			var average = await correctionResultsRepository
								.GetStudentTestInformationByTestAndStudent(test_Id, alu_id);

			return average;
		}

		/// <summary>
		/// Busca as médias de escolha por alternativa de cada item em uma prova
		/// </summary>
		/// <param name="test_Id">Id da prova</param>
		/// <param name="dre_id">Id da DRE</param>
		/// <param name="esc_id">Id da escola</param>
		/// <returns>Lista de projection com as médias(porcentagem) de escolha da DRE por alternativa de cada item da prova</returns>
		public async Task<List<ItemPercentageChoiceByAlternativeProjection>> GetItemPercentageChoiceByAlternative(long test_Id, long? discipline_id, Guid? dre_id, int? esc_id)
		{
			var itemsPercentageChoicePerAlternative = correctionResultsRepository
				.GetItemPercentageChoiceByAlternative(test_Id, discipline_id, dre_id, esc_id);

			return await itemsPercentageChoicePerAlternative;
		}

		public async Task<CorrectionResults> GetEntity(CorrectionResults entity)
		{
			var result = await correctionResultsRepository.FindOneAsync(entity);
			if(result is null)
            {
				string[] valores = entity._id.Split('_');
				return await GenerateCorrectionResults(new Guid(valores[0]), long.Parse(valores[1]), long.Parse(valores[2]));
			}

			return result;
		}

		public CorrectionResults GetResultFilterByDiscipline(CorrectionResults results, long? discipline_id)
		{
			if (discipline_id != null)
			{
				if (results.Answers != null)
				{
					results.Answers = results.Answers.FindAll(p => p.Discipline_Id == discipline_id);
					results.NumberAnswers = results.Answers.Count();
				}

				if (results.Statistics != null && results.Statistics.Averages != null)
				{
					results.Statistics.Averages = results.Statistics.Averages.FindAll(p => p.Discipline_Id == discipline_id);

				}

				if (results.Students != null)
				{
					int generalHits = 0;
					int qtdTotalAlternatives = 0;

					results.Students.ForEach(
							p =>
							{
								if (p.Alternatives != null)
								{
									p.Alternatives = p.Alternatives.FindAll(q => q.Discipline_Id == discipline_id);
									p.Hits = p.Alternatives.Count(q => q.Correct);
									generalHits += (int)(p.Hits.HasValue ? p.Hits : 0);
									int qtdAlternatives = p.Alternatives.Count();
									qtdTotalAlternatives += qtdAlternatives;
									p.Performance = Math.Round(qtdAlternatives > 0 ? ((double)p.Hits * 100 / qtdAlternatives) : 0, 2);
								}
							}
						);

					results.Statistics.GeneralHits = results.Students.Count > 0 ? generalHits / results.Students.Count : 0;
					results.Statistics.GeneralGrade = Math.Round(qtdTotalAlternatives > 0 ? ((double)generalHits * 100 / qtdTotalAlternatives) : 0, 2);
				}
			}

			if (results != null && results.Statistics != null)
			{
				if (double.IsNaN(results.Statistics.GeneralGrade))
				{
					results.Statistics.GeneralGrade = 0;
				}

				if (double.IsNaN(results.Statistics.GeneralHits))
				{
					results.Statistics.GeneralHits = 0;
				}
			}
			return results;
		}

		public async Task<bool> Delete(CorrectionResults entity)
		{
			return await correctionResultsRepository.Delete(entity);
		}

		public async Task<EntityFile> GetResultFile(Guid ent_id, long test_id, long tur_id, long? discipline_id, string separator, string virtualDirectory, string physicalDirectory)
		{
			EntityFile ret = new EntityFile();

			var results = await this.GetEntity(new CorrectionResults(ent_id, test_id, tur_id));
			results = GetResultFilterByDiscipline(results, discipline_id);

			StringBuilder itemsAlternatives = new StringBuilder();

			var report = new StringBuilder("Resultados Gerais da Turma");
			report.AppendLine();
			report.AppendLine();

			#region Informações das Questões
			foreach (var item in results.Answers)
				itemsAlternatives.AppendFormat("{0} ({1}{2}", item.Order + 1, item.RightChoice, separator);

			report.AppendLine(string.Format("Nº{0}Nome{0}Ausência{0}Acertos{0}Desempenho{0}{1}", separator, itemsAlternatives.ToString()));

			report.AppendLine();
			#endregion

			#region Informações da turma

			report.AppendLine(string.Format("{0}Toda a Turma{0}{0}{1}{0}{2}{0}{3}", separator, results.Statistics.GeneralHits, results.Statistics.GeneralGrade,
				string.Join(separator, results.Statistics.Averages.Select(i => i.Average))));

			#endregion

			#region Informações do aluno

			foreach (var item in results.Students)
				report.AppendLine(string.Format("{1}{0}{2}{0}{3}{0}{4}{0}{5}{0}{6}", separator,
					item.mtu_numeroChamada, item.alu_nome, item.AbsenceReason, item.Hits, item.Performance,
					item.Alternatives != null ? string.Join(separator, item.Alternatives.Select(a => a.Numeration.Substring(0, 1))) : string.Empty));

			#endregion

			string fileContent = report.ToString();

			if (!string.IsNullOrEmpty(fileContent))
			{
				var test = testBusiness.GetObject(test_id);
				var turma = turmaBusiness.GetWithTurno(tur_id);
				string fileName = string.Format("{0}_{1}_{2}", test.Description, turma.esc_id, turma.tur_codigo);

				byte[] buffer = System.Text.Encoding.Default.GetBytes(fileContent);
				string originalName = string.Format("{0}.csv", fileName);
				string name = string.Format("{0}.csv", Guid.NewGuid());
				string contentType = MimeType.CSV.GetDescription();

				var csvFiles = fileBusiness.GetAllFilesByType(EnumFileType.ExportCorrectionResult, DateTime.Now.AddDays(-1));
				if (csvFiles != null && csvFiles.Count() > 0)
				{
					fileBusiness.DeletePhysicalFiles(csvFiles.ToList(), physicalDirectory);
					fileBusiness.DeleteFilesByType(EnumFileType.ExportCorrectionResult, DateTime.Now.AddDays(-1));
				}

				ret = storage.Save(buffer, name, contentType, EnumFileType.ExportCorrectionResult.GetDescription(), virtualDirectory, physicalDirectory, out ret);
				if (ret.Validate.IsValid)
				{
					ret.ContentType = contentType;
					ret.OriginalName = StringHelper.Normalize(originalName);
					ret.OwnerId = tur_id;
					ret.ParentOwnerId = test_id;
					ret.OwnerType = (byte)EnumFileType.ExportCorrectionResult;
					ret = fileBusiness.Save(ret);
				}
			}
			else
			{
				ret.Validate.IsValid = false;
				ret.Validate.Type = ValidateType.alert.ToString();
				ret.Validate.Message = "Resultado da prova ainda não foi gerado.";
			}

			return ret;
		}

		public async Task<CorrectionResults> UnblockCorrection(long team_id, long test_id, SYS_Usuario user, EnumSYS_Visao visao)
		{
			var result = new CorrectionResults();
			result.Validate = this.ValidateUnblockCorrection(test_id, user.usu_id, visao);

			if (result.Validate.IsValid)
			{
				await this.Delete(new MongoEntities.CorrectionResults(user.ent_id, test_id, team_id));
				testSectionStatusCorrectionBusiness.SetStatusCorrection(test_id, team_id, EnumStatusCorrection.Processing);
				result.Validate.Type = ValidateType.Update.ToString();
				result.Validate.Message = "Prova salva com sucesso.";
			}

			return result;
		}

		public async Task<CorrectionResults> GenerateCorrectionResults(Guid ent_id, long test_id, long tur_id)
		{
			Parameter answerDuplicate = parameterBusiness.GetParamByKey("CODE_ALTERNATIVE_DUPLICATE", ent_id);
			Parameter answerEmpty = parameterBusiness.GetParamByKey("CODE_ALTERNATIVE_EMPTY", ent_id);

			MongoEntities.TestTemplate testTemplate = await testTemplateRepository.GetEntity(new MongoEntities.TestTemplate(ent_id, test_id));
			var answers = blockBusiness.GetTestQuestions(test_id);

			return await GenerateCorrectionResultsInsert(ent_id, test_id, tur_id, testTemplate, answers, answerDuplicate, answerEmpty);
		}

		public async Task<CorrectionResults> GenerateCorrectionResultsInsert(
			Guid ent_id, long test_id, long tur_id,
			MongoEntities.TestTemplate testTemplate,
			IEnumerable<StudentCorrectionAnswerGrid> answers,
			Parameter answerDuplicate,
			Parameter answerEmpty)
		{
			try
			{
				List<StudentCorrection> corrections = await studentCorrectionBusiness.GetByTest(test_id, tur_id);
				IEnumerable<long> aluMongoList = corrections.Select(i => i.alu_id);
				List<CorrectionStudentGrid> alunos = studentTestAbsenceReasonBusiness.GetByTestSection(test_id, tur_id, aluMongoList, true).ToList();
				var absences = studentTestAbsenceReasonBusiness.GetAbsencesByTestSection(test_id, tur_id);

				Guid dre_id = Guid.Empty;
				int esc_id = 0;

				if (alunos.Count > 0)
				{
					dre_id = alunos.FirstOrDefault().dre_id;
					esc_id = alunos.FirstOrDefault().esc_id;
				}

				await ProcessGrades(corrections, test_id, tur_id, ent_id, testTemplate, corrections.Count(), dre_id, esc_id);

				int qtdeItems = testTemplate.Items.Count;
				int qtdeAlunos = alunos.Count();
                int qtdeAusencias = alunos.Count(p => absences.Any(q => q.alu_id == p.alu_id));

				int qtdeLancamentos = 0;

				foreach (var item in corrections)
					qtdeLancamentos += item.Answers.Count;

				if ((qtdeLancamentos + (qtdeAusencias * qtdeItems)) < (qtdeItems * qtdeAlunos))
					testSectionStatusCorrectionBusiness.SetStatusCorrection(test_id, tur_id, EnumStatusCorrection.PartialSuccess);
				else
					testSectionStatusCorrectionBusiness.SetStatusCorrection(test_id, tur_id, EnumStatusCorrection.Success);

				SectionTestStats sectionStats = await sectionTestStatsBusiness.GetByTest(test_id, tur_id);

				var provaDisciplinas = testBusiness.GetDisciplineItemByTestId(test_id);

				var statsAluno = (from a in alunos
								  join s in corrections on a.alu_id equals s.alu_id
								  select new CorrectionResultsStudents()
								  {
									  alu_id = a.alu_id,
									  alu_nome = a.alu_nome,
									  Hits = s.Hits,
									  Performance = s.Grade,
									  mtu_numeroChamada = a.mtu_numeroChamada,
									  Alternatives = (from alternative in s.Answers
													  join answer in answers on alternative.Item_Id equals answer.Item_Id
													  orderby answer.Order
													  select new CorrectionResultsStudentsAnswers()
													  {
														  Alternative_Id = alternative.AnswerChoice > 0 ? answer.Alternatives.First(alt => alt.Id == alternative.AnswerChoice).Id : 0,
														  Item_Id = alternative.Item_Id,
														  Correct = alternative.Correct,
														  Numeration = alternative.StrikeThrough ? (answerDuplicate != null ? answerDuplicate.Value : "R") : (alternative.Empty ? (answerEmpty != null ? answerEmpty.Value : "N") :
															 answer.Alternatives.First(alt => alt.Id == alternative.AnswerChoice).Numeration.Replace(")", "")),
														  Revoked = testTemplate.Items.First(i => i.Item_Id == alternative.Item_Id).Revoked,
														  Discipline_Id = provaDisciplinas.First(p => p.Item_Id == alternative.Item_Id).Discipline_Id
													  }).ToList()
								  }).ToList();

				statsAluno.AddRange((from a in alunos
									 join abs in absences on a.alu_id equals abs.alu_id
									 select new CorrectionResultsStudents()
									 {
										 alu_id = a.alu_id,
										 AbsenceReason = abs.AbsenceReason.Description,
										 mtu_numeroChamada = a.mtu_numeroChamada,
										 alu_nome = a.alu_nome
									 }));

				statsAluno.AddRange((from a in alunos
									 where statsAluno.Count(alu => alu.mtu_numeroChamada == a.mtu_numeroChamada) == 0
									 select new CorrectionResultsStudents()
									 {
										 alu_id = a.alu_id,
										 mtu_numeroChamada = a.mtu_numeroChamada,
										 alu_nome = a.alu_nome
									 }));

				var escola = studentTestAbsenceReasonBusiness.GetEscIdDreIdByTeam(tur_id);

				int cur_id = alunos.First().cur_id;
				int crr_id = alunos.First().crr_id;
				int crp_id = alunos.First().crp_id;

				var entity = new CorrectionResults(ent_id, test_id, tur_id, escola.dre_id, escola.esc_id, cur_id, crr_id, crp_id)
				{
					Answers = testTemplate.Items.Select(i => new CorrectionResultsItems()
					{
						Alternative_Id = i.Alternative_Id,
						Item_Id = i.Item_Id,
						Order = i.Order,
						RightChoice = i.Numeration.Replace(")", ""),
						Revoked = i.Revoked,
						Discipline_Id = provaDisciplinas.First(p => p.Item_Id == i.Item_Id).Discipline_Id
					}).OrderBy(i => i.Order).ToList(),
					Statistics = new CorrectionResultsSectionStats()
					{
						GeneralGrade = sectionStats.GeneralGrade,
						GeneralHits = sectionStats.GeneralHits,
						Averages = (from s in sectionStats.Answers
									join i in testTemplate.Items on s.Item_Id equals i.Item_Id
									orderby i.Order
									select new Averages() { Item_Id = s.Item_Id, Average = s.Grade, Revoked = i.Revoked, Discipline_Id = provaDisciplinas.First(p => p.Item_Id == s.Item_Id).Discipline_Id }).ToList()
					},
					Students = statsAluno.OrderBy(n => n.mtu_numeroChamada).ToList()
				};

				entity.NumberAnswers = entity.Answers.Count;

				await Delete(new CorrectionResults(ent_id, test_id, tur_id));

				return await correctionResultsRepository.Insert(entity);
			}
			catch (Exception)
			{
				return new CorrectionResults();
			}
		}

		private async Task<SectionTestStats> ProcessGrades(List<StudentCorrection> corrections, long test_id, long tur_id, Guid ent_id, GestaoAvaliacao.MongoEntities.TestTemplate testTemplate, int qtdeAlunos, Guid dre_id, int esc_id)
		{
			//Totais para tirar dados da turma
			int acertos = 0;
			double desempenho = 0;

			//Total para pegar o desempenho por ítem
			Dictionary<long, int> desempenhoItem = new Dictionary<long, int>();
			var revokeds = requestRevokeBusiness.GetRevokedItemsByTest(test_id);
			int totalItem = testTemplate.Items.Count - revokeds.Count();

			if (revokeds.Count() > 0)
			{
				foreach (var item in testTemplate.Items.Where(i => revokeds.Contains(i.Item_Id)))
					item.Revoked = true;

				testTemplate = await testTemplateRepository.Replace(testTemplate);
			}

			foreach (var item in corrections)
			{
				item.Hits = item.Answers.Count(i => i.Correct && !revokeds.Contains(i.Item_Id));

				//arrendondar com uma casa após a virgula
				var grade = (item.Hits / (double)totalItem) * 100;
				item.Grade = Math.Round(grade, 2);

				acertos += item.Hits;
				desempenho += item.Grade;

				item.NumberAnswers = item.Answers.Count;

				foreach (var answer in item.Answers)
				{
					if (!desempenhoItem.ContainsKey(answer.Item_Id))
						desempenhoItem.Add(answer.Item_Id, 0);

					if (answer.Correct)
						desempenhoItem[answer.Item_Id]++;
				}

			}
			SectionTestStats entity = new SectionTestStats(test_id, tur_id, ent_id, dre_id, esc_id)
			{
				GeneralGrade = qtdeAlunos == 0 ? 0 : Math.Round((desempenho / (double)qtdeAlunos), 2),
				GeneralHits = qtdeAlunos == 0 ? 0 : Math.Round((acertos / (double)qtdeAlunos), 2),
				/* Desenvolvimento do Luis Maron para os relatorios
				As duas linhas acima substituidas pelas duas linhas abaixo
				GeneralHits = acertos,
				QtStudents = corrections.Count,
				*/
				Answers = desempenhoItem.Select(x => new SectionTestStatsAnswes() { Item_Id = x.Key, Grade = Math.Round((x.Value / (double)qtdeAlunos) * 100, 2) }).ToList()
			};

			entity.NumberAnswers = entity.Answers.Count;

			await studentCorrectionBusiness.Save(corrections);
			return await sectionTestStatsBusiness.Save(entity, ent_id);
		}

		#region Private methods

		private Validate ValidateUnblockCorrection(long test_id, Guid user, EnumSYS_Visao visao)
		{
			var valid = new Validate();
			var test = testBusiness.GetTestById(test_id);

			if (DateTime.Today < test.CorrectionStartDate || DateTime.Today > test.CorrectionEndDate)
				valid.Message = "Prova está fora do período de correção";
			else
			{
				if (test.TestType.Global)
				{
					if (visao != EnumSYS_Visao.Administracao)
						valid.Message = "Usuário não tem permissão para desbloquear esta prova";
				}
				else
				{
					if (test.UsuId != user)
						valid.Message = "Usuário não tem permissão para desbloquear esta prova";
				}
			}

			if (!string.IsNullOrEmpty(valid.Message))
			{
				valid.IsValid = false;

				if (valid.Code <= 0)
					valid.Code = 400;

				valid.Type = ValidateType.alert.ToString();
			}
			else
				valid.IsValid = true;

			return valid;
		}

		#endregion
	}
}
