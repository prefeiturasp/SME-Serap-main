using GestaoAvaliacao.API.App_Start;
using GestaoAvaliacao.API.Models;
using GestaoAvaliacao.Entities;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.MongoEntities.DTO;
using GestaoAvaliacao.Util;
using GestaoAvaliacao.WebProject.Facade;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using EntityFile = GestaoAvaliacao.Entities.File;

namespace GestaoAvaliacao.API.Controllers
{
	[CustomAuthorizationAttribute]
	[ApiExplorerSettings(IgnoreApi = true)]
	public class AnswerSheetController : ApiController
	{
		private readonly IAbsenceReasonBusiness _absenceReasonBusiness;
		private readonly IAnswerSheetBatchBusiness _batchBusiness;
		private readonly IAnswerSheetBatchFilesBusiness _batchFilesBusiness;
		private readonly IAnswerSheetBatchLogBusiness _batchLogBusiness;
		private readonly ITestBusiness _testBusiness;
		private readonly ICorrectionBusiness _correctionBusiness;
		private readonly IFileBusiness _fileBusiness;
		private readonly ITestSectionStatusCorrectionBusiness _testSectionStatusCorrectionBusiness;
		private readonly IParameterBusiness _parameterBusiness;

		public AnswerSheetController(IAbsenceReasonBusiness absenceReasonBusiness, IAnswerSheetBatchBusiness batchBusiness, IAnswerSheetBatchFilesBusiness batchFilesBusiness,
			IAnswerSheetBatchLogBusiness batchLogBusiness, ICorrectionBusiness correctionBusiness, ITestBusiness testBusiness, 
			IFileBusiness fileBusiness,
			ITestSectionStatusCorrectionBusiness testSectionStatusCorrectionBusiness, IParameterBusiness parameterBusiness)
		{
			_absenceReasonBusiness = absenceReasonBusiness;
			_batchBusiness = batchBusiness;
			_batchFilesBusiness = batchFilesBusiness;
			_batchLogBusiness = batchLogBusiness;
			_correctionBusiness = correctionBusiness;
			_testBusiness = testBusiness;
			_fileBusiness = fileBusiness;
			_testSectionStatusCorrectionBusiness = testSectionStatusCorrectionBusiness;
			_parameterBusiness = parameterBusiness;
		}

		/// <summary>
		/// Busca os arquivos (lote) por situação
		/// </summary>
		/// <param name="situation">situação do arquivo</param>
		/// <returns>Id, FileId, FileName, FileOriginalName, FileContentType, FilePath</returns>
		[Route("api/answersheet/getbatchfiles/{situation:int}")]
		[HttpGet]
		[ResponseType(typeof(AnswerSheetBatchFileResult))]
		public HttpResponseMessage GetBatchFiles(int situation)
		{
			return GetBatchFiles(situation, 0);
		}

		/// <summary>
		/// Busca os arquivos (lote) por situação
		/// </summary>
		/// <param name="situation">situação do arquivo</param>
		/// <param name="rows">Número de registros que a busca retorna (ex: SELECT TOP 100)</param>
		/// <returns>Id, FileId, FileName, FileOriginalName, FileContentType, FilePath</returns>
		[Route("api/answersheet/getbatchfiles/{situation:int}/{rows:int}")]
		[HttpGet]
		[ResponseType(typeof(AnswerSheetBatchFileResult))]
		public HttpResponseMessage GetBatchFiles(int situation, int rows)
		{
			try
			{
				var result = _batchFilesBusiness.GetBatchFiles((EnumBatchSituation)situation, rows);

				if (result != null)
				{
					return Request.CreateResponse(HttpStatusCode.OK, result);
				}
				else
				{
					return Request.CreateResponse(HttpStatusCode.NoContent, "Os arquivos não foram encontrados.");
				}
			}
			catch (Exception ex)
			{
				LogFacade.SaveError(ex);
				return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
			}
		}

		/// <summary>
		/// Busca os arquivos do lote
		/// </summary>
		/// <param name="id">Id do lote</param>
		/// <param name="status">false- Arquivos não enviados | true- Arquivos enviados</param>
		/// <returns>Retorna a lista de arquivos
		/// Retorno: Id, FileId, FileName, FileOriginalName, FileContentType, FilePath</returns>
		[Route("api/answersheet/getbatchfiles/{id:long}/{status:bool}")]
		[HttpGet]
		[ResponseType(typeof(AnswerSheetBatchFileResult))]
		public HttpResponseMessage GetBatchFiles(long id, bool status)
		{
			return GetBatchFiles(id, status, 0);
		}

		/// <summary>
		/// Busca os arquivos do lote
		/// </summary>
		/// <param name="id">Id do lote</param>
		/// <param name="status">false- Arquivos não enviados | true- Arquivos enviados</param>
		/// <param name="rows">Número de registros que a busca retorna (ex: SELECT TOP 100)</param>
		/// <returns>Retorna a lista de arquivos
		/// Retorno: Id, FileId, FileName, FileOriginalName, FileContentType, FilePath</returns>
		[Route("api/answersheet/getbatchfiles/{id:long}/{status:bool}/{rows:int}")]
		[HttpGet]
		[ResponseType(typeof(AnswerSheetBatchFileResult))]
		public HttpResponseMessage GetBatchFiles(long id, bool status, int rows)
		{
			try
			{
				IEnumerable<AnswerSheetBatchFileResult> result = null;
				if (id > 0)
				{
					//Busca os arquivos do lote
					result = _batchFilesBusiness.GetBatchFiles(id, status, rows);

					if (result != null)
					{
						//Carrega o lote
						AnswerSheetBatch batch = _batchBusiness.GetSimple(id);
						batch.Processing = EnumBatchProcessing.Processing;

						batch.AnswerSheetBatchFiles = result.Select(i => new AnswerSheetBatchFiles { Id = i.Id, Sent = true }).ToList();

						//Atualiza a situação do lote
						_batchBusiness.Update(batch.Id, batch);
						//Atualiza a flag de enviado dos arquivos do lote (significa que o OMR realizou a chamada do método para buscar os arquivos do lote)
						_batchFilesBusiness.UpdateSentList(batch.AnswerSheetBatchFiles);

						return Request.CreateResponse(HttpStatusCode.OK, result);
					}
					else
					{
						return Request.CreateResponse(HttpStatusCode.NoContent, "Os arquivos do lote não foram encontrados.");
					}
				}
				else
				{
					return Request.CreateResponse(HttpStatusCode.PreconditionFailed, "Id do lote não informado.");
				}
			}
			catch (Exception ex)
			{
				LogFacade.SaveError(ex);
				return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
			}
		}
	   
		/// <summary>
		/// Exclui os arquivos do lote
		/// </summary>
		/// <param name="id">Id do lote</param>
		/// <param name="status">false- Não exclui logicamente | true- Exclui logicamente</param>
		/// <returns>Retorna a quantidade de arquivos excluídos e os não excluídos ocasionado por algum problema
		/// HttpResponseMessage</returns>
		[Route("api/answersheet/deletebatchfiles/{id:long}/{status:bool}")]
		[HttpGet]
		public HttpResponseMessage DeleteBatchFiles(long id, bool status)
		{
			try
			{
				return DeleteFiles(id, status);
			}
			catch (Exception ex)
			{
				LogFacade.SaveError(ex);
				return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
			}
		}

		/// <summary>
		/// Salva o log de sucesso, erro ou inconsistência do arquivo
		/// </summary>
		/// <param name="result">
		/// <code>
		/// {
		///     "Id": (id do arquivo),
		///     "Status": (Success = 4, Error = 5, Warning = 6),
		///     "Description": (Descrição do erro),
		///     "StudentID": (id do aluno),
		///     "SectionId": (id da turma)
		/// }
		/// </code>
		/// </param>
		/// <param name="id">Id do arquivo</param>
		/// <param name="status">Success = 4, Error = 5, Warning = 6</param>
		/// <param name="description">Descrição do erro</param>
		/// <returns>HttpResponseMessage</returns>
		[Route("api/answersheet/postfileslog/")]
		[HttpPost]
		public HttpResponseMessage PostFilesLog([FromBody] BatchStudentResultModel result)
		{
			try
			{
				AnswerSheetBatchFiles studentFile = _batchFilesBusiness.Get(result.Id);
				if (studentFile != null)
				{
					studentFile.Section_Id = result.SectionId;
					studentFile.Student_Id = result.StudentID;
					studentFile.Situation = (EnumBatchSituation)result.Status;
					studentFile = _batchFilesBusiness.Update(studentFile);

					BatchFileResultModel fileLog = new BatchFileResultModel { Id = result.Id, Status = result.Status, Description = result.Description };
					if (!studentFile.Validate.IsValid)
					{
						fileLog.Status = (byte)EnumBatchSituation.Error;
						fileLog.Description = studentFile.Validate.Message;
					}
                    else if ((EnumBatchSituation)result.Status == EnumBatchSituation.Absent && result.TestId.HasValue && result.SectionId.HasValue && result.StudentID.HasValue)
                    {
                        // Caso o status seja ausente, salva o motivo default para o aluno.
                        Test test = _testBusiness.GetObjectWithTestType(result.TestId.Value);
                        Guid ent_id = test.TestType.EntityId;

                        var defaultAbsenceReason = _absenceReasonBusiness.GetDefault(ent_id);

                        StudentTestAbsenceReason absence = new StudentTestAbsenceReason()
                        {
                            Test_Id = result.TestId.Value,
                            tur_id = result.SectionId.Value,
                            alu_id = result.StudentID.Value,
                            AbsenceReason_Id = defaultAbsenceReason.Id
                        };
                        _correctionBusiness.SaveAbsenceReasonApi(absence, ent_id);
                    }

					return SaveFileLog(fileLog);
				}
				else
				{
					return Request.CreateResponse(HttpStatusCode.NoContent, "Arquivo não encontrado.");
				}
			}
			catch (Exception ex)
			{
				LogFacade.SaveError(ex);
				return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
			}
		}

		/// <summary>
		/// Associa arquivos identificados aos lotes da prova
		/// </summary>
		/// <param name="answerSheetBatchList">
		/// <code>
		/// [{ Test_Id: 123, AnswerSheetBatchFiles: [1,2,3,4]}]
		/// </code>
		/// </param>
		/// <returns></returns>
		[Route("api/answersheet/postbatchfiles/")]
		[HttpPost]
		public HttpResponseMessage PostBatchFiles([FromBody] IEnumerable<AnswerSheetBatch> answerSheetBatchList)
		{
			try
			{
				_batchFilesBusiness.UpdateBatchFilesIdentified(answerSheetBatchList);
				return Request.CreateResponse(HttpStatusCode.OK, "Arquivos associados com sucesso.");
			}
			catch (Exception ex)
			{
				LogFacade.SaveError(ex);
				return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
			}
		}

		/// <summary>
		/// Atualiza uma lista de arquivos
		/// </summary>
		/// <param name="fileList">Lista de arquivos do lote</param>
		/// <returns></returns>
		[Route("api/answersheet/updatefiles")]
		[HttpPost]
		public HttpResponseMessage UpdateFiles([FromBody] List<AnswerSheetBatchFiles> fileList)
		{
			try
			{
				_batchFilesBusiness.UpdateList(fileList);
				return Request.CreateResponse(HttpStatusCode.NoContent);
			}
			catch (Exception ex)
			{
				LogFacade.SaveError(ex);
				return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
			}
		}

		/// <summary>
		/// Salva o status de sucesso ou falha do lote
		/// </summary>
		/// <param name="id">Id do lote</param>
		/// <param name="status">Pending = 1, Processing = 2, Success = 3, Failure = 4, Retry = 5, Initiate = 6</param>
		/// <param name="description">Descrição do erro</param>
		/// <returns>HttpStatusCode</returns>
		[Route("api/answersheet/postbatchlog/{id:long}/{status:int}")]
		[HttpPost]
		public HttpResponseMessage PostBatchLog(long id, int status)
		{
			try
			{
				if (id > 0)
				{
					AnswerSheetBatch batch = UpdateProcessing(id, (EnumBatchProcessing)status);
					if (batch != null && batch.Validate.IsValid)
					{
						if(batch.Test_Id > 0)
							_testBusiness.UpdateTestProcessedCorrection(batch.Test_Id, false);

						return Request.CreateResponse(HttpStatusCode.OK);
					}
					else
					{
						return Request.CreateResponse(HttpStatusCode.NoContent, "Erro ao atualizar a situação do lote.");
					}
				}
				else
				{
					return Request.CreateResponse(HttpStatusCode.PreconditionFailed, "Id do lote não informado.");
				}
			}
			catch (Exception ex)
			{
				LogFacade.SaveError(ex);
				return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
			}
		}

		/// <summary>
		/// Grava o resultado do lote (correção)
		/// </summary>
		/// <param name="result">
		/// <code>
		/// {
		///     "Batch_Id": 10011,
		///     "Test_Id": 5,
		///     "Section_Id": 97808,
		///     "Students": [{
		///         "Id": 1186642,
		///         "NumberId": 1,
		///         "File_Id": 97,
		///         "Items": [{
		///             "Id": 1,
		///             "AlternativeId": 0,
		///             "EmptyAlternative": false,
		///             "DuplicateAlternative": false
		///         }]
		///     }]
		/// }
		/// </code>
		/// </param>
		/// <returns>HttpResponseMessage</returns>
		[Route("api/answersheet/postbatchresult/")]
		[HttpPost]
		public async Task<HttpResponseMessage> PostBatchResult([FromBody]BatchResultModel result)
		{
			Validate valid = new Validate();

			try
			{
				valid = ValidateBatchResult(result);
				if (valid.IsValid)
				{
					Test test = _testBusiness.GetObjectWithTestType(result.Test_Id);
					if (test != null)
					{
						if (test.TestType != null)
						{
							result.Ent_Id = test.TestType.EntityId;

							AnswerSheetBatch batch = _batchBusiness.GetSimple(result.Batch_Id);
							int countFiles = _batchFilesBusiness.GetFilesCount(result.Batch_Id);
							if (batch != null && countFiles > 0)
							{
								var listoToUpdate = new List<AnswerSheetBatchFiles>();
								AnswerSheetBatchResult info = null;
								if (batch.OwnerEntity.Equals(EnumAnswerSheetBatchOwner.Test))
								{
									info = _batchBusiness.GetSectionInformation(result.Section_Id);
								}

								var defaultAbsenceReason = _absenceReasonBusiness.GetDefault(result.Ent_Id);
								var taskTestTemplate = await _correctionBusiness.GetTestTemplate(result.Test_Id, result.Ent_Id);

								foreach (StudentModel student in result.Students)
								{
									if (batch.OwnerEntity.Equals(EnumAnswerSheetBatchOwner.Section))
									{
										if(batch.BatchType == EnumAnswerSheetBatchType.NumberID)
											student.Id = _batchBusiness.GetStudentId(result.Section_Id, mtu_numeroChamada: student.NumberId);
										else if(batch.BatchType == EnumAnswerSheetBatchType.QRCode)
											student.Id = _batchBusiness.GetStudentId(result.Section_Id, alu_id: student.Id);

										if (student.Id <= 0)
										{
											_testSectionStatusCorrectionBusiness.SetStatusCorrection(result.Test_Id, result.Section_Id, EnumStatusCorrection.Processing);
											continue;
										}
									}

									//File_Id do model StudentModel corresponde à coluna Id da tabela AnswerSheetBatchFiles
									AnswerSheetBatchFiles batchFile = _batchFilesBusiness.GetFile(student.File_Id, 0);
									if (batchFile != null)
									{
										batchFile.Student_Id = student.Id;
										batchFile.Section_Id = result.Section_Id;

										if (batch.OwnerEntity.Equals(EnumAnswerSheetBatchOwner.Test) && info != null)
										{
											batchFile.School_Id = info.SchoolId;
											batchFile.SupAdmUnit_Id = info.SupAdmUnitId;
										}

										// deveria ser um adapter ao invés de instanciar diretamente
										var testModel = new TestDTO
										{
											test_Id = result.Test_Id,
											tur_Id = result.Section_Id,
											ent_Id = result.Ent_Id
										};

										StudentTestAbsenceReason absence = new StudentTestAbsenceReason()
										{
											Test_Id = testModel.test_Id,
											tur_id = testModel.tur_Id,
											alu_id = student.Id
										};

										if (student.Absent)
										{
                                            // Caso o aluno esteja ausente, altera a situação da correção da prova dele para ausente
                                            batchFile.Situation = EnumBatchSituation.Absent;

                                            absence.AbsenceReason_Id = defaultAbsenceReason.Id;
											await _correctionBusiness.SaveAbsenceReasonApi(absence, testModel.ent_Id);
										}
										else {
											absence.AbsenceReason_Id = 0;
											await _correctionBusiness.SaveAbsenceReasonApi(absence, testModel.ent_Id);
											await _correctionBusiness.SaveCorrectionApi(student.Id, student.Items, testModel, taskTestTemplate);
										}

										listoToUpdate.Add(batchFile);
									}
									else
									{
										continue;
									}
								}

								//Atualiza os alunos dos arquivos do lote
								_batchFilesBusiness.UpdateList(listoToUpdate);

								await _correctionBusiness.FinalizeAutomaticCorrection(result.Section_Id, test.Id, test.TestType.EntityId, taskTestTemplate);

								string responseMessage;
								if (ShouldDeleteBatchFiles(result.Ent_Id))
								{
									var deleteResponse = DeleteFiles(result.Batch_Id, result.exclusionLogic);
									responseMessage = string.Format("Exclusão dos arquivos do lote | HttpStatusCode: {0} | Message: {1}", deleteResponse.StatusCode, deleteResponse.ReasonPhrase);
								}
								else
								{
									responseMessage = "Arquivos do lote não foram excluidos.";
								}

								return Request.CreateResponse(HttpStatusCode.OK, "Lote processado com sucesso. " + responseMessage);
							}
							else
							{
								if (result.Test_Id > 0 && result.Section_Id > 0)
									_testSectionStatusCorrectionBusiness.SetStatusCorrection(result.Test_Id, result.Section_Id, EnumStatusCorrection.Processing);

								return Request.CreateResponse(HttpStatusCode.PreconditionFailed, "Lote e arquivos não encontrados.");
							}
						}
						else
						{
							if (result.Test_Id > 0 && result.Section_Id > 0)
								_testSectionStatusCorrectionBusiness.SetStatusCorrection(result.Test_Id, result.Section_Id, EnumStatusCorrection.Processing);

							return Request.CreateResponse(HttpStatusCode.PreconditionFailed, "Disciplina da prova não encontrada.");
						}
					}
					else
					{
						return Request.CreateResponse(HttpStatusCode.PreconditionFailed, "Prova não encontrada.");
					}
				}
				else
				{
					if (result.Test_Id > 0 && result.Section_Id > 0)
						_testSectionStatusCorrectionBusiness.SetStatusCorrection(result.Test_Id, result.Section_Id, EnumStatusCorrection.Processing);
					return Request.CreateResponse(HttpStatusCode.PreconditionFailed, valid.Message);
				}
			}
			catch (Exception ex)
			{
				if(result.Test_Id > 0 && result.Section_Id > 0)
					_testSectionStatusCorrectionBusiness.SetStatusCorrection(result.Test_Id, result.Section_Id, EnumStatusCorrection.Processing);

				LogFacade.SaveError(ex);
				return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
			}
		}

		private bool ShouldDeleteBatchFiles(Guid ent_Id)
		{
			var shouldDelete = _parameterBusiness.GetByKey("DELETE_BATCH_FILES", ent_Id);
			return shouldDelete != null && bool.Parse(shouldDelete.Value);
		}

		/// <summary>
		/// Exclui os arquivos do lote
		/// </summary>
		/// <param name="id">Id do lote</param>
		/// <param name="status">false- Não exclui logicamente | true- Exclui logicamente</param>
		/// <returns>Retorna a quantidade de arquivos excluídos e os não excluídos ocasionado por algum problema
		/// HttpResponseMessage</returns>
		private HttpResponseMessage DeleteFiles(long id, bool status)
		{
			try
			{
				if (id > 0)
				{
					//Busca os arquivos do lote
					IEnumerable<AnswerSheetBatchFiles> files = _batchFilesBusiness.GetFiles(id, true);

					if (files != null && files.Count() > 0)
					{
						List<long> filesId = files.Select(i => i.File_Id).ToList();
						if (status)
							_batchFilesBusiness.DeleteList(files.ToList());

						IEnumerable<EntityFile> result = _fileBusiness.DeleteFiles(filesId, status, Guid.Empty);
						if (result != null && result.Count() > 0)
						{
							int deleted = result.Where(i => i.Validate.IsValid).Count();
							int failed = result.Where(i => !i.Validate.IsValid).Count();

							return Request.CreateResponse(HttpStatusCode.OK, string.Format("{0} arquivo(s) excluído(s) com sucesso e ocorreu erro na exclusão de {1} arquivo(s).", deleted, failed));
						}
						else
						{
							return Request.CreateResponse(HttpStatusCode.NoContent, "A exclusão não foi executada.");
						}
					}
					else
					{
						return Request.CreateResponse(HttpStatusCode.NoContent, "Os arquivos do lote não foram encontrados.");
					}
				}
				else
				{
					return Request.CreateResponse(HttpStatusCode.PreconditionFailed, "Id do lote não informado.");
				}
			}
			catch (Exception ex)
			{
				LogFacade.SaveError(ex);
				return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
			}
		}

		/// <summary>
		/// Atualiza a situação do lote
		/// </summary>
		/// <param name="id">Id do lote</param>
		/// <param name="status">EnumBatchProcessing: Pending = 1,Processing = 2,Success = 3,Failure = 4,Retry = 5</param>
		/// <returns>Retorna o lote</returns>
		private AnswerSheetBatch UpdateProcessing(long id, EnumBatchProcessing status)
		{
			//Carrega o lote
			AnswerSheetBatch batch = _batchBusiness.Get(id);
			if (batch != null)
			{
				//Atualiza a situação do lote
				batch.Processing = status;
				batch = _batchBusiness.Update(id, batch);
			}

			return batch;
		}

		private HttpResponseMessage SaveFileLog(BatchFileResultModel entity)
		{
			try
			{
				if (entity.Id > 0)
				{
					//Busca o log do arquivo
					AnswerSheetBatchLog log = _batchLogBusiness.GetByBatchFile_Id(entity.Id);

					//Se existe log, atualiza a situação
					if (log != null && log.Id > 0)
					{
						log.AnswerSheetBatchFile = null;
						log.AnswerSheetBatchFile_Id = entity.Id;
						log.Situation = (EnumBatchSituation)entity.Status;
						log.Description = entity.Description;

						log = _batchLogBusiness.Update(log.Id, log);
					}
					else
					{
						//Se não existe log, insere
						log = new AnswerSheetBatchLog
						{
							Id = 0,
							AnswerSheetBatchFile = null,
							AnswerSheetBatchFile_Id = entity.Id,
							Situation = (EnumBatchSituation)entity.Status,
							Description = entity.Description
						};

						log = _batchLogBusiness.Save(log);
					}

					if (log.Validate.IsValid)
					{
                        return Request.CreateResponse(HttpStatusCode.OK);
					}
					else
					{
						return Request.CreateResponse(HttpStatusCode.NoContent, string.Format("Erro ao {0} o log do arquivo.", log.Id > 0 ? "alterar" : "salvar"));
					}
				}
				else
				{
					return Request.CreateResponse(HttpStatusCode.PreconditionFailed, "Id do arquivo não informado.");
				}
			}
			catch (Exception ex)
			{
				LogFacade.SaveError(ex);
				return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
			}
		}

		private Validate ValidateBatchResult(BatchResultModel entity)
		{
			Validate valid = new Util.Validate();
			valid.Message = null;

			if (entity.Test_Id <= 0)
				valid.Message = "Id da prova não foi informado.";

			if (entity.Section_Id <= 0)
				valid.Message += "<br/>Id da turma não foi informado.";

			if (entity.Batch_Id <= 0)
				valid.Message += "<br/>Id do lote não foi informado.";

			if (entity.Students == null || (entity.Students != null && entity.Students.Count() <= 0))
				valid.Message += "<br/>Lista de alunos vazia.";
			else
			{
				var studentId = entity.Students.Any(i => i.Id <= 0);
				var numberId = entity.Students.Any(i => i.NumberId <= 0);
				if (studentId && numberId)
					valid.Message += "<br/>Id do aluno não foi informado.";

				var fileId = entity.Students.Any(i => i.File_Id <= 0);
				if (fileId)
					valid.Message += "<br/>Id do arquivo do aluno não foi informado.";

				foreach (StudentModel student in entity.Students)
				{
					List<ItemModelDTO> items = student.Items;

					if (items == null || (items != null && items.Count() <= 0))
					{
						valid.Message += "<br/>Lista de itens do aluno vazia.";
						//Se achou pelo menos um problema, sai do for
						break;
					}
				}
			}

			if (!string.IsNullOrEmpty(valid.Message))
			{
				string br = "<br/>";
				valid.Message = valid.Message.TrimStart(br.ToCharArray());

				valid.IsValid = false;

				if (valid.Code <= 0)
					valid.Code = 400;

				valid.Type = ValidateType.alert.ToString();
			}
			else
				valid.IsValid = true;

			return valid;
		}
	}
}
