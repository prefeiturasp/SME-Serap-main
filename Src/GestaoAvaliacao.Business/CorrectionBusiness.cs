using GestaoAvaliacao.Business.Adapters;
using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Entities.DTO;
using GestaoAvaliacao.Entities.DTO.StudentsTestSent;
using GestaoAvaliacao.Entities.DTO.Tests;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.IBusiness.StudentsTestSent;
using GestaoAvaliacao.IRepository;
using GestaoAvaliacao.MongoEntities;
using GestaoAvaliacao.MongoEntities.DTO;
using GestaoAvaliacao.Util;
using GestaoEscolar.IBusiness;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CoreSSO = MSTech.CoreSSO.Entities;

namespace GestaoAvaliacao.Business
{
	public class CorrectionBusiness : ICorrectionBusiness
	{
		private readonly ICorrectionRepository _correctionRepository;
		private readonly ITestBusiness _testBusiness;
		private readonly ITestTemplateRepository _testTemplateRepository;
		private readonly IStudentCorrectionBusiness _studentCorrectionBusiness;
		private readonly IBlockBusiness _blockBusiness;
		private readonly IStudentTestAbsenceReasonBusiness _studentTestAbsenceReasonBusiness;
		private readonly ISectionTestStatsBusiness _sectionTestStatsBusiness;
		private readonly ICorrectionResultsBusiness _correctionResultsBusiness;
		private readonly ITestSectionStatusCorrectionBusiness _testSectionStatusCorrectionBusiness;
		private readonly IRequestRevokeBusiness _requestRevokeBusiness;
		private readonly IResponseChangeLogBusiness _responseChangeLogBusiness;
        private readonly IStudentTestSentBusiness _studentTestSentBusiness;

        public CorrectionBusiness(ICorrectionRepository correctionRepository, ITestBusiness testBusiness,
			ITestTemplateRepository testTemplateRepository, IStudentCorrectionBusiness studentCorrectionBusiness, IBlockBusiness blockBusiness,
			IStudentTestAbsenceReasonBusiness studentTestAbsenceReasonBusiness,
			ISectionTestStatsBusiness sectionTestStatsBusiness, ICorrectionResultsBusiness correctionResultsBusiness, ITUR_TurmaBusiness turmaBusiness,
			ITestSectionStatusCorrectionBusiness testSectionStatusCorrectionBusiness, IRequestRevokeBusiness requestRevokeBusiness,
			IResponseChangeLogBusiness responseChangeLogBusiness, IStudentTestSentBusiness studentTestSentBusiness)
		{
			_correctionRepository = correctionRepository;
			_testBusiness = testBusiness;
			_testTemplateRepository = testTemplateRepository;
			_studentCorrectionBusiness = studentCorrectionBusiness;
			_blockBusiness = blockBusiness;
			_studentTestAbsenceReasonBusiness = studentTestAbsenceReasonBusiness;
			_sectionTestStatsBusiness = sectionTestStatsBusiness;
			_correctionResultsBusiness = correctionResultsBusiness;
			_testSectionStatusCorrectionBusiness = testSectionStatusCorrectionBusiness;
			_requestRevokeBusiness = requestRevokeBusiness;
			_responseChangeLogBusiness = responseChangeLogBusiness;
            _studentTestSentBusiness = studentTestSentBusiness;
        }

		#region Write

		public async Task<StudentCorrection> SaveCorrectionApi(long studentId, List<ItemModelDTO> itemModelList, TestDTO testModel, MongoEntities.TestTemplate testTemplate)
		{
			await _testSectionStatusCorrectionBusiness.SetCorrectionStatusAsync(testModel.test_Id, testModel.tur_Id, EnumStatusCorrection.Processing);
			itemModelList.ForEach(a => a.ValidateAlternative());

			var answerList = ItemModelAdapter.ToAnswer(itemModelList);
			answerList.ForEach(a => a.Correct = testTemplate.Items.Any(i => i.Item_Id == a.Item_Id && i.Alternative_Id == a.AnswerChoice));

			var studentAnswers = await GetStudentAnswer(testModel.test_Id, testModel.tur_Id, studentId, testModel.ent_Id);

			foreach (Answer answer in answerList)
			{
				var retornoLog = await _responseChangeLogBusiness.Save(answer, studentId, testModel.test_Id, testModel.tur_Id, testModel.ent_Id, Guid.Empty, false, studentAnswers, 0, 0);
			}

			var retorno = await _studentCorrectionBusiness.SaveAPI(answerList, studentId, testModel);

			var retornoTempCorrection = _testSectionStatusCorrectionBusiness.GetTempCorrection(testModel.ent_Id, testModel.test_Id, testModel.tur_Id);

			retorno.Validate = new Validate() { IsValid = true, Message = "Nota do usuário salva com sucesso", Type = ValidateType.Save.ToString() };

			return retorno;
		}

		public async Task<StudentCorrection> SaveCorrectionAsync(long test_id, long alu_id, long tur_id, IEnumerable<AnswerModelDto> chosenAlternatives,
			Guid ent_id, Guid usuId, Guid pesId, EnumSYS_Visao visao, int ordemItem = 0)
        {
			var valid = ValidateTest(test_id, tur_id, alu_id, usuId, pesId, visao);
			if (!valid.IsValid) return new StudentCorrection() { Validate = valid };

			var testTemplate = await GetTestTemplate(test_id, ent_id);
			if(testTemplate is null)
				return new StudentCorrection { Validate = new Validate { IsValid = false, Message = "Template adprova não encontrado.", Type = ValidateType.Save.ToString() } };

			var answers = chosenAlternatives
				.Select(x => new Answer
				{
					AnswerChoice = x.AlternativeId,
					Correct = testTemplate.Items.Any(i => i.Item_Id == x.ItemId && i.Alternative_Id == x.AlternativeId),
					Item_Id = x.ItemId,
					Empty = false,
					StrikeThrough = false,
					Automatic = false
				})
				.ToList();

			var taskLogs = Task.Run(async () =>
			{
				var studentAnswers = await GetStudentAnswer(test_id, tur_id, alu_id, ent_id);
				await _responseChangeLogBusiness.SaveAsync(answers, alu_id, test_id, tur_id, ent_id, usuId, manual: false, studentAnswers, 0, 0);
			});

			var taskCorrectionStatus = _testSectionStatusCorrectionBusiness.SetCorrectionStatusAsync(test_id, tur_id, EnumStatusCorrection.Processing);
			var taskCorrection = _studentCorrectionBusiness.SaveAsync(answers, alu_id, test_id, tur_id, ent_id, api: false, ordemItem, false);
			var taskTempCorrectionResult = _testSectionStatusCorrectionBusiness.GetTempCorrection(ent_id, test_id, tur_id);

			await Task.WhenAll(taskCorrectionStatus, taskLogs, taskCorrection, taskTempCorrectionResult);

			var retorno = taskCorrection.Result;
			retorno.Validate = new Validate() { IsValid = true, Message = "Nota do usuário salva com sucesso", Type = ValidateType.Save.ToString() };

			return retorno;
		}

		public async Task<StudentCorrection> SaveCorrection(long alu_id, long alternative_id, long item_id, bool n, bool r, long test_id,
			long tur_id, Guid ent_id, Guid usuId, Guid pesId, EnumSYS_Visao visao, bool manual, bool api = false, int ordemItem = 0, bool provaEletronica = false)
		{
			if (!api)
			{
				Validate valid = ValidateTest(test_id, tur_id, alu_id, usuId, pesId, visao);

				if (!valid.IsValid)
					return new StudentCorrection() { Validate = valid };
			}

			var testTemplate = await GetTestTemplate(test_id, ent_id);

			var answer = new Answer
			{
				AnswerChoice = alternative_id,
				Correct = testTemplate.Items.Any(i => i.Item_Id == item_id && i.Alternative_Id == alternative_id),
				Item_Id = item_id,
				Empty = n,
				StrikeThrough = r,
				Automatic = !manual
			};

			var taskLogs = Task.Run(async () =>
			{
				var studentAnswers = await GetStudentAnswer(test_id, tur_id, alu_id, ent_id);
				await _responseChangeLogBusiness.Save(answer, alu_id, test_id, tur_id, ent_id, usuId, manual, studentAnswers, 0, 0);
			});

			var taskCorrectionStatus = _testSectionStatusCorrectionBusiness.SetCorrectionStatusAsync(test_id, tur_id, EnumStatusCorrection.Processing);
			var taskCorrection = _studentCorrectionBusiness.Save(answer, alu_id, test_id, tur_id, ent_id, api, ordemItem, false);
			var taskTempCorrectionResult = _testSectionStatusCorrectionBusiness.GetTempCorrection(ent_id, test_id, tur_id);
			await Task.WhenAll(taskCorrectionStatus, taskLogs, taskCorrection, taskTempCorrectionResult);

			var retorno = taskCorrection.Result;
			retorno.Validate = new Validate() { IsValid = true, Message = "Nota do usuário salva com sucesso", Type = ValidateType.Save.ToString() };

			return retorno;
		}

		public async Task<StudentTestAbsenceReason> SaveAbsenceReason(StudentTestAbsenceReason entity, Guid usuId, Guid pesId, EnumSYS_Visao visao, Guid ent_id)
		{
			var statusCorrection = _testSectionStatusCorrectionBusiness.Get(entity.Test_Id, entity.tur_id);

			if (statusCorrection == null)
			{
				TestSectionStatusCorrection testSectionStatusCorrection = new TestSectionStatusCorrection()
				{
					StatusCorrection = EnumStatusCorrection.Processing,
					Test_Id = entity.Test_Id,
					tur_id = entity.tur_id
				};
				_testSectionStatusCorrectionBusiness.Save(testSectionStatusCorrection);
				statusCorrection = testSectionStatusCorrection;
			}
			else if (statusCorrection.StatusCorrection == EnumStatusCorrection.Pending)
				_testSectionStatusCorrectionBusiness.SetStatusCorrection(entity.Test_Id, entity.tur_id, EnumStatusCorrection.Processing);

			SchoolDTO escola = _studentTestAbsenceReasonBusiness.GetEscIdDreIdByTeam(entity.tur_id);

			Validate valid = ValidateTest(entity.Test_Id, entity.tur_id, 0, usuId, pesId, visao);
			if (!valid.IsValid)
				return new StudentTestAbsenceReason() { Validate = valid };

			await _studentCorrectionBusiness.Delete(new StudentCorrection(entity.Test_Id, entity.tur_id, entity.alu_id, ent_id, escola.dre_id, escola.esc_id));

			await _testSectionStatusCorrectionBusiness.GetTempCorrection(ent_id, entity.Test_Id, entity.tur_id);

			return _studentTestAbsenceReasonBusiness.Save(entity, usuId, ent_id, true);
		}

		public async Task<StudentTestAbsenceReason> SaveAbsenceReasonApi(StudentTestAbsenceReason entity, Guid ent_id)
		{
			var statusCorrection = _testSectionStatusCorrectionBusiness.Get(entity.Test_Id, entity.tur_id);

			if (statusCorrection == null)
			{
				TestSectionStatusCorrection testSectionStatusCorrection = new TestSectionStatusCorrection()
				{
					StatusCorrection = EnumStatusCorrection.Processing,
					Test_Id = entity.Test_Id,
					tur_id = entity.tur_id
				};
				_testSectionStatusCorrectionBusiness.Save(testSectionStatusCorrection);
				statusCorrection = testSectionStatusCorrection;
			}
			else if (statusCorrection.StatusCorrection == EnumStatusCorrection.Pending)
				_testSectionStatusCorrectionBusiness.SetStatusCorrection(entity.Test_Id, entity.tur_id, EnumStatusCorrection.Processing);

			SchoolDTO escola = _studentTestAbsenceReasonBusiness.GetEscIdDreIdByTeam(entity.tur_id);

			if (entity.AbsenceReason_Id == 0)
			{
				await _testSectionStatusCorrectionBusiness.GetTempCorrection(ent_id, entity.Test_Id, entity.tur_id);
				return _studentTestAbsenceReasonBusiness.Remove(entity);
			}
			else
			{
				await _studentCorrectionBusiness.Delete(new StudentCorrection(entity.Test_Id, entity.tur_id, entity.alu_id, ent_id, escola.dre_id, escola.esc_id));
				StudentTestAbsenceReason studentTestAbsenceReason = _studentTestAbsenceReasonBusiness.Save(entity, Guid.Empty, ent_id, false);
				await _testSectionStatusCorrectionBusiness.GetTempCorrection(ent_id, entity.Test_Id, entity.tur_id);
				return studentTestAbsenceReason;
			}


		}

		public async Task<Adherence> FinalizeCorrection(long team, long test_id, CoreSSO.SYS_Usuario user, EnumSYS_Visao visao)
		{
			Adherence retorno = new Adherence();
			retorno.Validate = this.CanEditTest(test_id, team, user, visao);

			if (retorno.Validate.IsValid)
			{
				List<StudentCorrection> corrections = await _studentCorrectionBusiness.GetByTest(test_id, team);
				IEnumerable<long> aluMongoList = corrections.Select(i => i.alu_id);
				var alunos = _studentTestAbsenceReasonBusiness.GetByTestSection(test_id, team, aluMongoList, true).ToList();
				var absences = _studentTestAbsenceReasonBusiness.GetAbsencesByTestSection(test_id, team);

				Guid dre_id = Guid.Empty;
				int esc_id = 0;

				if (alunos.Count > 0)
				{
					dre_id = alunos.FirstOrDefault().dre_id;
					esc_id = alunos.FirstOrDefault().esc_id;
				}

				if (corrections.Count == 0 && absences.Count() == 0)
				{
					retorno.Validate.IsValid = false;
					retorno.Validate.Type = ValidateType.alert.ToString();
					retorno.Validate.Message = "Não foram corrigidas as respostas de todos os alunos.";

					return retorno;
				}
				var templateTest = await this.GetTestTemplate(test_id, user.ent_id);

				retorno.Validate = this.ValidateFinalize(alunos, corrections, templateTest, test_id, team);

				if (retorno.Validate.IsValid)
				{
					//Processar notas
					await this.ProcessGrades(corrections, test_id, team, user.ent_id, templateTest, corrections.Count(), dre_id, esc_id);

					await _correctionResultsBusiness.GenerateCorrectionResults(user.ent_id, test_id, team);

                    _testSectionStatusCorrectionBusiness.SetStatusCorrection(test_id, team, EnumStatusCorrection.Success);
                }
            }

			return retorno;
		}

		/// <summary>
		/// Envia prova eletrônica para a fila de processamento.
		/// </summary>
		/// <param name="tur_id"></param>
		/// <param name="test_id"></param>
		/// <param name="user"></param>
		/// <param name="visao"></param>
		/// <returns></returns>
		public async Task<FinalizeCorrectionDto> SendElectronicTestAsync(FinalizeCorrectionDto dto, CancellationToken cancellationToken)
        {
			if(dto is null)
            {
				dto = new FinalizeCorrectionDto();
				dto.AddError("Não foram informados os dados para entrega da prova. Por favor tente novamente.");
				return dto;
			}

			var studentTestSent = await _studentTestSentBusiness.SaveAsync(dto.TestId, dto.TurId, dto.AluId, dto.EntId, dto.Visao, dto.UsuId, cancellationToken);
			if (studentTestSent is null)
			{
				dto.AddError("Não foi possível concluir a entrega da prova. Por favor tente novamente.");
				return dto;
			}

			var studentCorrection = await _studentCorrectionBusiness.FinalizeStudentCorrectionAsync(dto.TestId, dto.TurId, dto.AluId, dto.EntId);
			if(studentCorrection is null)
            {
				dto.AddError("Não foi possível concluir a entrega da prova. Por favor tente novamente.");
				return dto;
			}

			if (!studentTestSent.Validate.IsValid) dto.AddError(studentTestSent.Validate.Message);
			if (!studentCorrection.Validate.IsValid) dto.AddError(studentCorrection.Validate.Message);
			return dto;
		}

		/// <summary>
		/// Processa a correção da prova eletrônica.
		/// </summary>
		/// <param name="tur_id"></param>
		/// <param name="test_id"></param>
		/// <param name="user"></param>
		/// <param name="visao"></param>
		/// <returns></returns>
		public async Task<Adherence> FinalizeCorrectionElectronicTest(long tur_id, long test_id, long alu_id, CoreSSO.SYS_Usuario user, EnumSYS_Visao visao)
		{
			Adherence retorno = new Adherence();

			List<StudentCorrection> lstStudentCorrection = new List<StudentCorrection>();

			StudentCorrection studentCorrection = await _studentCorrectionBusiness.Get(alu_id, test_id, tur_id, user.ent_id);
			var alunos = _studentTestAbsenceReasonBusiness.GetByTestSectionByAluId(test_id, tur_id, alu_id, true).ToList();
			var absences = _studentTestAbsenceReasonBusiness.GetAbsencesByTestSection(test_id, tur_id);

			Guid dre_id = Guid.Empty;
			int esc_id = 0;

			if (alunos.Count > 0)
			{
				dre_id = alunos.FirstOrDefault(p => p.alu_id == alu_id).dre_id;
				esc_id = alunos.FirstOrDefault(p => p.alu_id == alu_id).esc_id;
			}

			var templateTest = await this.GetTestTemplate(test_id, user.ent_id);

			foreach (var item in templateTest.Items)
			{
				if (studentCorrection == null || !studentCorrection.Answers.Exists(p => p.Item_Id == item.Item_Id))
				{
					Answer answer = new Answer()
					{
						AnswerChoice = 0,
						Correct = false,
						Item_Id = item.Item_Id,
						Empty = true,
						StrikeThrough = false,
						Automatic = true
					};

					var studentAnswers = await GetStudentAnswer(test_id, tur_id, alu_id, user.ent_id);

					await _responseChangeLogBusiness.Save(answer, alu_id, test_id, tur_id, user.ent_id, user.usu_id, true, studentAnswers, 0, 0);

					await _studentCorrectionBusiness.Save(answer, alu_id, test_id, tur_id, user.ent_id, false, 0, true);

					await _testSectionStatusCorrectionBusiness.GetTempCorrection(user.usu_id, test_id, tur_id);
				}
			}

			studentCorrection = await _studentCorrectionBusiness.Get(alu_id, test_id, tur_id, user.ent_id);

			studentCorrection.provaFinalizada = true;

			lstStudentCorrection.Add(studentCorrection);

			//Processar notas
			await ProcessGrades(lstStudentCorrection, test_id, tur_id, user.ent_id, templateTest, lstStudentCorrection.Count(), dre_id, esc_id);

			await _correctionResultsBusiness.GenerateCorrectionResults(user.ent_id, test_id, tur_id);

			return retorno;
		}

		/// <summary>
		/// Método utilizado para finalização de correção de uma turma com correção automática, regras diferentes das utilizados na correção manual
		/// </summary>
		/// <param name="team">Id da turma</param>
		/// <param name="test_id">Id da prova</param>
		/// <returns></returns>
		public async Task<Adherence> FinalizeAutomaticCorrection(long team, long test_id, Guid EntityId, MongoEntities.TestTemplate testTemplate)
		{
			var retorno = new Adherence();

			var corrections = await _studentCorrectionBusiness.GetByTest(test_id, team);

			IEnumerable<long> aluMongoList = corrections.Select(i => i.alu_id);
			var alunos = _studentTestAbsenceReasonBusiness.GetByTestSection(test_id, team, aluMongoList, true).ToList();

			Guid dre_id = Guid.Empty;
			int esc_id = 0;

			if (alunos.Count > 0)
			{
				dre_id = alunos.FirstOrDefault().dre_id;
				esc_id = alunos.FirstOrDefault().esc_id;
			}

			await this.ProcessGrades(corrections, test_id, team, EntityId, testTemplate, corrections.Count(), dre_id, esc_id);

			var validate = this.ValidateFinalize(alunos, corrections, testTemplate, test_id, team);

			if (validate.IsValid && await _studentCorrectionBusiness.CountInconsistency(test_id, team) == 0)
				_testSectionStatusCorrectionBusiness.SetStatusCorrection(test_id, team, EnumStatusCorrection.Success);
			else
				_testSectionStatusCorrectionBusiness.SetStatusCorrection(test_id, team, EnumStatusCorrection.PartialSuccess);

			await _correctionResultsBusiness.GenerateCorrectionResults(EntityId, test_id, team);

			retorno.Validate.IsValid = true;
			retorno.Validate.Message = "Correção da turma finalizada com sucesso.";

			return retorno;
		}

		#endregion

		#region Read

		public async Task<IEnumerable<StudentCorrectionAnswerGrid>> GetStudentAnswer(long test_id, long tur_id, long alu_id, Guid ent_id)
		{
			var answers = _blockBusiness.GetTestQuestions(test_id);

			StudentCorrection studentCorrection = await _studentCorrectionBusiness.Get(alu_id, test_id, tur_id, ent_id);

			if (studentCorrection != null)
			{
				var merge = (from a in answers
							 join s in studentCorrection.Answers on a.Item_Id equals s.Item_Id
							 select new { a.Item_Id, s.AnswerChoice, s.Correct, s.Empty, s.StrikeThrough });

				foreach (var item in merge)
				{
					var answer = answers.First(i => i.Item_Id == item.Item_Id);

					if (item.AnswerChoice > 0)
						answer.Alternatives.First(a => a.Id == item.AnswerChoice).Selected = true;
					else if (item.Empty)
						answer.Null = item.Empty;
					else
						answer.StrikeThrough = item.StrikeThrough;
				}
			}

			return answers;
		}

		public IEnumerable<SelectedSection> LoadOnlySelectedSectionPaginate(ref Pager pager, StudentResponseFilter filter)
		{
			Test test = _testBusiness.GetObject(filter.Test_Id);

			if (test != null)
			{
				filter.TestType_Id = test.TestType_Id;
				filter.AllAdhered = test.AllAdhered;
				filter.CorrectionEndDate = test.CorrectionEndDate;
			}

			if ((EnumSYS_Visao)filter.vis_id == EnumSYS_Visao.Individual)
			{
				filter.usu_id = null;
				filter.vis_id = null;
				filter.sis_id = null;
			}
			else
			{
				filter.pes_id = null;
			}

			IEnumerable<SelectedSection> list = this._correctionRepository.LoadOnlySelectedSectionPaginate(ref pager, filter);

			return list;
		}

		public async Task<CorrectionResults> GetResultCorrectionGrid(long tur_id, long test_id, long? discipline_id, Guid ent_id)
		{
			TestSectionStatusCorrection ad = _testSectionStatusCorrectionBusiness.Get(test_id, tur_id);

			if (ad != null && (ad.StatusCorrection == EnumStatusCorrection.Success || ad.StatusCorrection == EnumStatusCorrection.PartialSuccess))
			{
				CorrectionResults results = await _correctionResultsBusiness.GetEntity(new CorrectionResults(ent_id, test_id, tur_id));
				return _correctionResultsBusiness.GetResultFilterByDiscipline(results, discipline_id);
			}
			else
				return null;

		}

		public async Task<CorrectionResults> InsertCorretionResults(long tur_id, long test_id, Guid ent_id, MongoEntities.TestTemplate testTemplate,
			IEnumerable<StudentCorrectionAnswerGrid> answers, Parameter answerDuplicate, Parameter answerEmpty)
		{
			TestSectionStatusCorrection ad = _testSectionStatusCorrectionBusiness.Get(test_id, tur_id);

			if ((ad != null) && (ad.StatusCorrection == EnumStatusCorrection.Success || ad.StatusCorrection == EnumStatusCorrection.PartialSuccess || ad.StatusCorrection == EnumStatusCorrection.Processing))
			{
				return await _correctionResultsBusiness.GenerateCorrectionResultsInsert(ent_id, test_id, tur_id, testTemplate, answers, answerDuplicate, answerEmpty);
			}
			else
				return null;

		}

		public async Task<List<CorrectionStudentGrid>> GetGridCorrection(long tur_id, long test_id)
		{
			var corrections = await _studentCorrectionBusiness.GetByTest(test_id, tur_id);
			IEnumerable<long> aluMongoList = corrections.Select(i => i.alu_id);

			var grid = _studentTestAbsenceReasonBusiness.GetByTestSection(test_id, tur_id, aluMongoList, false).ToList();
			var answers = _blockBusiness.GetTestQuestions(test_id);

			foreach (var student in grid)
			{
				student.QtdeItem = answers.Count();

				var correction = corrections.FirstOrDefault(c => c.alu_id == student.alu_id);

				student.Items = answers.Select(a => (StudentCorrectionAnswerGrid)a.Clone()).ToList();

				if (correction != null)
				{
					student.Correcteds = correction != null ? correction.Answers.Count : 0;
					student.Automatic = correction.Automatic;

					var merge = (from a in student.Items
								 join s in correction.Answers on a.Item_Id equals s.Item_Id
								 select new { a.Item_Id, s.AnswerChoice, s.Correct, s.Empty, s.StrikeThrough });

					if (correction.provaFinalizada.HasValue && correction.provaFinalizada.Value)
					{
						student.status = StatusProvaEletronica.Finalizada;
					}
					else
					{
						student.status = StatusProvaEletronica.EmAndamento;
					}

					foreach (var item in merge)
					{
						var answer = student.Items.First(i => i.Item_Id == item.Item_Id);

						if (item.AnswerChoice > 0)
							answer.Alternatives.First(a => a.Id == item.AnswerChoice).Selected = true;
						else if (item.Empty)
							answer.Null = item.Empty;
						else
							answer.StrikeThrough = item.StrikeThrough;
					}
				}
				else
				{
					student.status = StatusProvaEletronica.NaoIniciada;
				}
			}

			return grid;
		}

		public async Task<MongoEntities.TestTemplate> GetTestTemplate(long test_id, Guid ent_id)
		{
			var result = await _testTemplateRepository.FindOneAsync(new MongoEntities.TestTemplate(ent_id, test_id));
			if (result is null) return await GenerateTestTemplate(test_id, ent_id);
			return result;
		}

		#endregion

		#region Private methods

		private void SetCorrection(long test_id, long tur_id)
		{
			var statusCorrection = _testSectionStatusCorrectionBusiness.Get(test_id, tur_id);

			if (statusCorrection == null)
			{
				TestSectionStatusCorrection entity = new TestSectionStatusCorrection()
				{
					StatusCorrection = EnumStatusCorrection.Processing,
					Test_Id = test_id,
					tur_id = tur_id
				};
				_testSectionStatusCorrectionBusiness.Save(entity);
			}
			else if (statusCorrection.StatusCorrection == EnumStatusCorrection.Pending)
				_testSectionStatusCorrectionBusiness.SetStatusCorrection(test_id, tur_id, EnumStatusCorrection.Processing);
		}

		private async Task<SectionTestStats> ProcessGrades(List<StudentCorrection> corrections, long test_id, long tur_id, Guid ent_id, GestaoAvaliacao.MongoEntities.TestTemplate testTemplate, int qtdeAlunos, Guid dre_id, int esc_id)
		{
			//Totais para tirar dados da turma
			int acertos = 0;
			double desempenho = 0;

			//Total para pegar o desempenho por ítem
			Dictionary<long, int> desempenhoItem = new Dictionary<long, int>();
			var revokeds = _requestRevokeBusiness.GetRevokedItemsByTest(test_id);
			int totalItem = testTemplate.Items.Count - revokeds.Count();

			if (revokeds.Count() > 0)
			{
				foreach (var item in testTemplate.Items.Where(i => revokeds.Contains(i.Item_Id)))
					item.Revoked = true;

				testTemplate = await _testTemplateRepository.Replace(testTemplate);
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
				GeneralGrade = Math.Round((desempenho / (double)qtdeAlunos), 2),
				GeneralHits = Math.Round((acertos / (double)qtdeAlunos), 2),
				/* Desenvolvimento do Luis Maron para os relatorios
				As duas linhas acima substituidas pelas duas linhas abaixo
				GeneralHits = acertos,
				QtStudents = corrections.Count,
				*/
				Answers = desempenhoItem.Select(x => new SectionTestStatsAnswes() { Item_Id = x.Key, Grade = Math.Round((x.Value / (double)qtdeAlunos) * 100, 2) }).ToList()
			};

			entity.NumberAnswers = entity.Answers.Count;

			await _studentCorrectionBusiness.Save(corrections);
			return await _sectionTestStatsBusiness.Save(entity, ent_id);
		}

		private async Task<MongoEntities.TestTemplate> GenerateTestTemplate(long test_id, Guid ent_id)
		{
			var test = _correctionRepository.GetTestTemplate(test_id);

			MongoEntities.TestTemplate entity = new MongoEntities.TestTemplate(ent_id, test_id)
			{
				Test_Id = test_id,
				Items = test.Select(x => new MongoEntities.Item()
				{
					Alternative_Id = x.Alternative_Id,
					Item_Id = x.Item_Id,
					Numeration = x.Numeration,
					Order = x.Order
				}).ToList()
			};

			return await _testTemplateRepository.Insert(entity);
		}

		private Validate ValidateFinalize(IEnumerable<CorrectionStudentGrid> alunos,
			List<StudentCorrection> corrections, MongoEntities.TestTemplate templateTest, long test_id, long tur_id)
		{
			Validate valid = new Validate();

			var ausentes = _studentTestAbsenceReasonBusiness.StudentAbsencesByTestSection(test_id, tur_id);

			var alunosCorrigidos = corrections.Select(c => c.alu_id).Union(ausentes);

			//Verificar se a quantidades de alunos com notas salvas é diferente do total de alunos na turma
			if (alunos.Where(a => !alunosCorrigidos.Contains(a.alu_id)).Count() > 0)
				valid.Message = "Não foram informados todos os dados necessários para finalizar a correção.";
			else
			{
				//verificar se a quantidade de alternativas lançadas dos alunos está correta
				//Quantidade total de lançamentos é qtde de alunos * qtde de items na prova
				int qtdeItems = templateTest.Items.Count, qtdeAlunos = corrections.Count();

				int qtdeLancamentos = 0;

				foreach (var item in corrections)
					qtdeLancamentos += item.Answers.Count;

				if (qtdeLancamentos != (qtdeItems * qtdeAlunos))
					valid.Message = "Não foram informados todos os dados necessários para finalizar a correção.";
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

		private Validate ValidateTest(long test_id, long tur_id, long alu_id, Guid usuId, Guid pesId, EnumSYS_Visao visao)
		{
			var test = _testBusiness.GetObjectWithTestType(test_id);
			Validate retorno = new Validate();

			if (!test.ElectronicTest)
			{
				if (!Compare.ValidateBetweenShortDates(Convert.ToDateTime(test.CorrectionStartDate), Convert.ToDateTime(test.CorrectionEndDate)))
					retorno.Message = "A prova não está no período de correção.";
				//else if (visao != EnumSYS_Visao.UnidadeAdministrativa)
				//{
				//    if (visao != EnumSYS_Visao.Administracao && test.UsuId != usuId && !_turmaBusiness.ValidateTeacherSection(tur_id, pesId))
				//        retorno.Message = "Usuário não possui permissão para realizar essa ação.";
				//}
				else
				{
					var statusCorrection = _testSectionStatusCorrectionBusiness.Get(test_id, tur_id);

					if (statusCorrection != null && statusCorrection.StatusCorrection == EnumStatusCorrection.Success)
						retorno.Message = "A correção da prova já está concluída.";
				}

				if (alu_id > 0)
				{
					var absenceReason = _studentTestAbsenceReasonBusiness.GetByTestStudent(test_id, tur_id, alu_id);
					if (absenceReason != null && absenceReason.AbsenceReason_Id > 0)
						retorno.Message = "A correção da prova do aluno não é permitida, pois já existe um motivo de ausência selecionado.";
				}
			}

			if (!string.IsNullOrEmpty(retorno.Message))
			{
				retorno.IsValid = false;

				if (retorno.Code <= 0)
					retorno.Code = 400;

				retorno.Type = ValidateType.alert.ToString();
			}
			else
				retorno.IsValid = true;

			return retorno;
		}

		private Validate CanEditTest(long test_id, long tur_id, CoreSSO.SYS_Usuario user, EnumSYS_Visao visao)
		{
			var test = _testBusiness.GetTestById(test_id);
			var valid = new Validate();

			//if (visao != EnumSYS_Visao.UnidadeAdministrativa && (visao != EnumSYS_Visao.Administracao && test.UsuId != user.usu_id && !_turmaBusiness.ValidateTeacherSection(tur_id, user.pes_id)))
			//{
			//    valid.Message = "Usuário não possui permissão para realizar essa ação.";
			//}

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
