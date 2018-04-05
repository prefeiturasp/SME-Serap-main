using GestaoAvaliacao.App_Start;
using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Entities.DTO;
using GestaoAvaliacao.Entities.Enumerator;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.MongoEntities;
using GestaoAvaliacao.Util;
using GestaoAvaliacao.WebProject.Facade;
using GestaoEscolar.Entities;
using GestaoEscolar.IBusiness;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using EntityFile = GestaoAvaliacao.Entities.File;

namespace GestaoAvaliacao.Controllers
{
	[Authorize]
	[AuthorizeModule]
	public class AnswerSheetController : Controller
	{
		private readonly IAnswerSheetBusiness answerSheetBusiness;
		private readonly IAnswerSheetBatchBusiness batchBusiness;
		private readonly IAnswerSheetBatchFilesBusiness batchFilesBusiness;
		private readonly ITestBusiness testBusiness;
		private readonly IESC_EscolaBusiness escolaBusiness;
		private readonly ITUR_TurmaBusiness turmaBusiness;
		private readonly IACA_TipoTurnoBusiness turnoBusiness;
		private readonly IFileBusiness fileBusiness;
		private readonly IBookletBusiness bookletBusiness;
		private readonly IAnswerSheetLotBusiness answerSheetLotBusiness;
		private readonly IAnswerSheetBatchQueueBusiness answerSheetBatchQueueBusiness;
        private readonly ISectionTestGenerateLotBusiness sectionTestGenerateLotBusiness;

        public AnswerSheetController(IESC_EscolaBusiness escolaBusiness, ITUR_TurmaBusiness turmaBusiness, IAnswerSheetBusiness answerSheetBusiness, ITestBusiness testBusiness,
			IAnswerSheetBatchBusiness batchBusiness, IAnswerSheetBatchFilesBusiness batchFilesBusiness, IACA_TipoTurnoBusiness turnoBusiness, IFileBusiness fileBusiness,
			IBookletBusiness bookletBusiness, IAnswerSheetLotBusiness answerSheetLotBusiness, IAnswerSheetBatchQueueBusiness answerSheetBatchQueueBusiness, ISectionTestGenerateLotBusiness sectionTestGenerateLotBusiness)
		{
			this.answerSheetBusiness = answerSheetBusiness;
			this.testBusiness = testBusiness;
			this.turmaBusiness = turmaBusiness;
			this.escolaBusiness = escolaBusiness;
			this.batchBusiness = batchBusiness;
			this.batchFilesBusiness = batchFilesBusiness;
			this.turnoBusiness = turnoBusiness;
			this.fileBusiness = fileBusiness;
			this.bookletBusiness = bookletBusiness;
			this.answerSheetLotBusiness = answerSheetLotBusiness;
			this.answerSheetBatchQueueBusiness = answerSheetBatchQueueBusiness;
            this.sectionTestGenerateLotBusiness = sectionTestGenerateLotBusiness;
        }

		#region View

		public ActionResult IndexBatchDetails()
		{
			return View();
		}

        public ActionResult IndexBatchDetailsLot()
        {
            return View();
        }

        public ActionResult IndexBatchDetailsLotFiles()
        {
            return View();
        }

        public ActionResult AnswerSheetLot()
        {
            if (SessionFacade.UsuarioLogado.Grupo.vis_id != (int)EnumSYS_Visao.Administracao)
                Response.Redirect("~/");

			return View();
		}

		public ActionResult IndexAnswerSheetStudent()
		{
			return View();
		}

		#endregion

		#region Read

		[HttpGet]
		public JsonResult GetAuthorize(long batch_id = 0, long test_id = 0, int school_id = 0, long team_id = 0)
		{
			try
			{
				AnswerSheetBatch batch = null;
				if (batch_id > 0)
				{
					batch = batchBusiness.Get(batch_id);
					if (batch != null)
					{
						test_id = batch.Test_Id;
						school_id = batch.School_Id != null ? (int)batch.School_Id : 0;
						team_id = batch.Section_Id != null ? (long)batch.Section_Id : 0;
					}
				}
				else
				{
					AnswerSheetBatchFilter filter = new AnswerSheetBatchFilter();
					filter.UserId = SessionFacade.UsuarioLogado.Usuario.usu_id;
					filter.PesId = SessionFacade.UsuarioLogado.Usuario.pes_id;
					filter.CoreVisionId = SessionFacade.UsuarioLogado.Grupo.vis_id;
					filter.CoreSystemId = Constants.IdSistema;
					filter.EntId = SessionFacade.UsuarioLogado.Usuario.ent_id;
					filter.CoreGroupId = SessionFacade.UsuarioLogado.Grupo.gru_id;
					filter.TestId = test_id;
					filter.SchoolId = school_id;
					filter.SectionId = team_id;

					batch = batchBusiness.Find(filter);
				}

				if (test_id > 0)
				{
					var test = testBusiness.GetTestById(test_id);
					if (test != null)
					{
						ESC_Escola escola = null;
						TUR_Turma turma = null;
						ACA_TipoTurno turno = null;

						if (school_id > 0)
						{
							escola = escolaBusiness.Get(school_id);
						}
						else if (team_id > 0)
						{
							turma = turmaBusiness.Get(team_id);

							escola = escolaBusiness.Get(turma.esc_id);

							if (turma.ttn_id != null)
							{
								turno = turnoBusiness.Get((int)turma.ttn_id);
							}
						}

						bool blockAccess = false;
						if (team_id > 0 && (EnumSYS_Visao)SessionFacade.UsuarioLogado.Grupo.vis_id == EnumSYS_Visao.Individual &&
							test.UsuId != SessionFacade.UsuarioLogado.Usuario.usu_id &&
							!turmaBusiness.ValidateTeacherSection(team_id, SessionFacade.UsuarioLogado.Usuario.pes_id))
						{
							blockAccess = true;
						}

						var dados = new
						{
							test = new
							{
								Id = test.Id,
								Owner = test.UsuId.Equals(SessionFacade.UsuarioLogado.Usuario.usu_id),
								Description = !string.IsNullOrEmpty(test.Description) ? test.Description : "Prova sem nome",
								FrequencyApplication = EnumHelper.GetFrenquencyApplication(test.FrequencyApplication, test.TestType.FrequencyApplication, true, true),
								Discipline = !string.IsNullOrEmpty(test.Discipline.Description) ? test.Discipline.Description : "Sem disciplina"
							},
							school = new
							{
								Id = escola != null ? escola.esc_id : 0,
								Description = escola != null ? escola.esc_nome : null
							},
							team = new
							{
								Id = turma != null ? turma.tur_id : 0,
								Description = turma != null ? turma.tur_codigo + (turno != null ? " - " + turno.ttn_nome : string.Empty) : null
							},
							batch = batch,
							blockAccess = blockAccess
						};

						return Json(new { success = true, dados = dados }, JsonRequestBehavior.AllowGet);
					}
				}

				return Json(new { success = false, type = ValidateType.alert.ToString(), message = "Não foi possível carregar os dados." }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				LogFacade.SaveError(ex);
				return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao carregar dados." }, JsonRequestBehavior.AllowGet);
			}
		}

		[HttpGet]
		public JsonResult GetProcessingList()
		{
			try
			{
				var ret = Enum.GetValues(typeof(EnumBatchProcessing)).Cast<EnumBatchProcessing>().Select(v => new
				{
					Id = (int)v,
					Description = EnumHelper.GetDescriptionFromEnumValue(v)
				}).ToList();

				if (ret != null)
				{
					return Json(new { success = true, lista = ret }, JsonRequestBehavior.AllowGet);
				}
				else
					return Json(new { success = false, type = ValidateType.alert.ToString(), message = "Opções de processamento não encontradas." }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				LogFacade.SaveError(ex);
				return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar opções de processamento." }, JsonRequestBehavior.AllowGet);
			}
		}

		[HttpGet]
		public JsonResult GetSituationList()
		{
			try
			{
				var ret = Enum.GetValues(typeof(EnumBatchSituation)).Cast<EnumBatchSituation>().Select(v => new
				{
					Id = (int)v,
					Description = EnumHelper.GetDescriptionFromEnumValue(v)
				}).ToList();

				if (ret != null)
				{
					return Json(new { success = true, lista = ret }, JsonRequestBehavior.AllowGet);
				}
				else
					return Json(new { success = false, type = ValidateType.alert.ToString(), message = "Opções de processamento não encontradas." }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				LogFacade.SaveError(ex);
				return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar opções de processamento." }, JsonRequestBehavior.AllowGet);
			}
		}

		#region AnswerSheetBatchFiles

		[HttpGet]
		public JsonResult FindBatch(AnswerSheetBatchFilter filter)
		{
			try
			{
				filter = GetFilter(filter);

				AnswerSheetBatch batch = batchBusiness.Find(filter);
				if (batch != null && filter.TestId > 0 && filter.SectionId >= 0)
				{
					filter.BatchId = batch.Id;
				}

				AnswerSheetBatchFileCountResult batchInfo = batchFilesBusiness.GetCountBatchInformation(filter);
				var result = new
				{
					success = true,
					entity = filter.TestId > 0 ? batch : new AnswerSheetBatch(),
					batch = batchInfo != null ? new
					{
						CreateDate = batch != null ? batch.CreateDate.ToShortDateString() : null,
						Total = batchInfo.Total,
						Errors = batchInfo.Errors,
						Warnings = batchInfo.Warnings,
						PendingIdentification = batchInfo.PendingIdentification,
						Success = batchInfo.Success,
						Absents = batchInfo.Absents,
						NotIdentified = batchInfo.NotIdentified,
						Pending = batchInfo.Pending
					} : null,
					QRCode = batch != null && batch.BatchType.Equals(EnumAnswerSheetBatchType.QRCode),
					blockUpload = batch != null && batch.Processing.Equals(EnumBatchProcessing.Processing)
				};

				return Json(result, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				LogFacade.SaveError(ex);
				return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar o lote." }, JsonRequestBehavior.AllowGet);
			}
		}

		[HttpGet]
		[Paginate]
		public JsonResult GetBatchAnswerSheetDetail(AnswerSheetBatchFilter filter)
		{
			try
			{
				Pager pager = this.GetPager();
				filter = GetFilter(filter);

                IEnumerable<AnswerSheetBatchResult> result = batchFilesBusiness.SearchBatchFiles(ref pager, filter, filter.AluNome, filter.Turma);

                if (result != null)
                {
                    var ret = result.Select(entity => new
                    {
                        Id = entity.Id,
                        Description = entity.Description,
                        DRE = new
                        {
                            Id = entity.SupAdmUnitId,
                            Description = entity.SupAdmUnitName,
                            Sigla = entity.UadSigla,
                            Test_Id = entity.Test_Id
                        },
                        School = new
                        {
                            Id = entity.SchoolId,
                            Description = entity.SchoolName
                        },
                        Team = new
                        {
                            Id = entity.SectionId,
                            Description = entity.SectionName
                        },
                        Student = new
                        {
                            Id = entity.StudentId,
                            Description = entity.StudentName
                        },
                        File = new
                        {
                            Id = entity.FileId,
                            Description = entity.FileName,
                            Resolution = entity.Resolution
                        },
                        CreateDate = entity.CreateDate.ToString("dd/MM/yyyy HH:mm:ss", new CultureInfo("pt-BR")),
                        UpdateDate = entity.UpdateDate.ToString("dd/MM/yyyy HH:mm:ss", new CultureInfo("pt-BR")),
                        Situation = entity.Situation,
                        FilePath = entity.FilePath
                    }).ToList();

					return Json(new { success = true, lista = ret, filterDateUpdate = filter.FilterDateUpdate }, JsonRequestBehavior.AllowGet);
				}
				else
				{
					return Json(new { success = true, lista = "" }, JsonRequestBehavior.AllowGet);
				}
			}
			catch (Exception ex)
			{
				LogFacade.SaveError(ex);
				return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar lote de folhas de respostas pesquisado." }, JsonRequestBehavior.AllowGet);
			}
		}

		[HttpGet]
		public void GetStudentFile(long testId, long studentId, long sectionId)
		{
			bool redirect = false;
			try
			{
				EntityFile entityFile = batchFilesBusiness.GetStudentFile(testId, studentId, sectionId, ApplicationFacade.PhysicalDirectory, ApplicationFacade.VirtualDirectory, SessionFacade.UsuarioLogado.Usuario);

				if (entityFile.Validate.IsValid)
				{
					string decodedUrl = string.Empty;
					string path = entityFile.Path;
					string fileName = entityFile.OriginalName;

					if (entityFile.Id > 0)
					{
						string filePath = new Uri(path).AbsolutePath.Replace("Files/", string.Empty);
						string physicalPath = string.Concat(ApplicationFacade.PhysicalDirectory, filePath.Replace("/", "\\"));
						decodedUrl = HttpUtility.UrlDecode(physicalPath);

						if (System.IO.File.Exists(decodedUrl))
						{
							System.IO.FileStream fs = System.IO.File.Open(decodedUrl, System.IO.FileMode.Open);
							byte[] btFile = new byte[fs.Length];
							fs.Read(btFile, 0, Convert.ToInt32(fs.Length));
							fs.Close();

							Response.Clear();
							Response.ContentType = "application/octet-stream";
							Response.AddHeader("Content-disposition", "attachment; filename=\"" + fileName + "\"");
							Response.BinaryWrite(btFile);
							Response.End();
							redirect = true;
						}
					}
					else
					{
						decodedUrl = HttpUtility.UrlDecode(path);

						int bytesToRead = 10000;
						byte[] buffer = new Byte[bytesToRead];

						HttpWebRequest fileReq = (HttpWebRequest)HttpWebRequest.Create(decodedUrl);
						HttpWebResponse fileResp = (HttpWebResponse)fileReq.GetResponse();
						Stream stream = fileResp.GetResponseStream();

						Response.Clear();
						Response.ContentType = "application/octet-stream";
						Response.AddHeader("Content-Disposition", "attachment; filename=\"" + fileName + "\"");

						int length;
						do
						{
							if (Response.IsClientConnected)
							{
								length = stream.Read(buffer, 0, bytesToRead);
								Response.OutputStream.Write(buffer, 0, length);
								Response.Flush();
								buffer = new Byte[bytesToRead];
							}
							else
							{
								length = -1;
							}
						} while (length > 0);

						if (stream != null)
						{
							stream.Close();
						}

						Response.End();
						redirect = true;
					}
				}
			}
			catch (Exception ex)
			{
				LogFacade.SaveError(ex);
			}

			if (!redirect && Request.UrlReferrer != null && !string.IsNullOrEmpty(Request.UrlReferrer.PathAndQuery))
				Response.Redirect(Request.UrlReferrer.PathAndQuery, false);
		}

		[HttpGet]
		public JsonResult ExportAnswerSheetData(AnswerSheetBatchFilter filter)
		{
			EntityFile ret = new EntityFile();

			try
			{
				filter = GetFilter(filter);

				string separator = ";";
				Parameter param = ApplicationFacade.Parameters.FirstOrDefault(p => p.Key.Equals(EnumParameterKey.CHAR_SEP_CSV.GetDescription()));
				if (param != null)
					separator = param.Value;

				ret = batchFilesBusiness.ExportAnswerSheetData(filter, separator, ApplicationFacade.VirtualDirectory, ApplicationFacade.PhysicalDirectory);
			}
			catch (Exception ex)
			{
				LogFacade.SaveError(ex);
				ret.Validate.Type = ValidateType.error.ToString();
				ret.Validate.IsValid = false;
				ret.Validate.Message = "Erro ao obter os dados dos arquivos.";
			}

			return Json(new { success = ret.Validate.IsValid, type = ret.Validate.Type, message = ret.Validate.Message, file = ret }, JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		public JsonResult SendToProcessing(AnswerSheetBatchFilter filter)
		{
			AnswerSheetBatch entity = null;

			try
			{
				entity = batchFilesBusiness.SendToProcessing(filter);
			}
			catch (Exception ex)
			{
				entity = new AnswerSheetBatch();
				entity.Validate.IsValid = false;
				entity.Validate.Type = ValidateType.error.ToString();
				entity.Validate.Message = "Erro ao enviar o lote para processamento.";

				LogFacade.SaveError(ex);
			}

			return Json(new { success = entity.Validate.IsValid, type = entity.Validate.Type, message = entity.Validate.Message }, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region AnswerSheetLot

		[HttpGet]
		[Paginate]
		public JsonResult SearchTestLot(AnswerSheetLotFilter filter)
		{
			try
			{
				Pager pager = this.GetPager();
				var ret = answerSheetLotBusiness.GetTestLot(ref pager, filter);

				if (ret != null && ret.Count() > 0)
				{
					var result = ret.Select(entity => new
					{
						Id = entity.Id,
						TestCode = entity.TestCode,
						TestTypeDescription = entity.TestTypeDescription,
						Description = entity.Description,
						FileId = entity.FileId,
						StateExecution = (byte)entity.StateExecution > 0 ? (byte)entity.StateExecution : (byte)EnumServiceState.NotRequest,
						Type = (byte)EnumAnswerSheetBatchOwner.Test
					});

					return Json(new { success = true, lista = result }, JsonRequestBehavior.AllowGet);
				}
				else
				{
					return Json(new { success = true, lista = "" }, JsonRequestBehavior.AllowGet);
				}
			}
			catch (Exception ex)
			{
				LogFacade.SaveError(ex);
				return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar as provas." }, JsonRequestBehavior.AllowGet);
			}
		}

		[HttpGet]
		[Paginate]
		public JsonResult SearchLotList(AnswerSheetLotFilter filter)
		{
			try
			{
				Pager pager = this.GetPager();
				var ret = answerSheetLotBusiness.GetLotList(filter, ref pager);

				if (ret != null && ret.Count() > 0)
				{
					var result = ret.Select(entity => new
					{
						Lot_Id = entity.Id,
						CreateDate = entity.CreateDate.ToShortDateString(),
						TestCount = answerSheetLotBusiness.GetTestCount(entity.Id),
						TestList = answerSheetLotBusiness.GetTestList(entity.Id).Select(i => new { Id = i.Id, Description = i.Description }),
						RequestDate = entity.RequestDate != null ? Convert.ToDateTime(entity.RequestDate).ToString("dd/MM/yyyy HH:mm:ss", new CultureInfo("pt-BR")) : " - ",
						StateExecution = (byte)entity.StateExecution,
						Type = (byte)EnumAnswerSheetBatchOwner.School
					});

					return Json(new { success = true, lista = result }, JsonRequestBehavior.AllowGet);
				}
				else
				{
					return Json(new { success = true, lista = "" }, JsonRequestBehavior.AllowGet);
				}
			}
			catch (Exception ex)
			{
				LogFacade.SaveError(ex);
				return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar os lotes." }, JsonRequestBehavior.AllowGet);
			}
		}

		[HttpGet]
		public JsonResult GetAnswerSheetLotSituationList()
		{
			try
			{
				var ret = Enum.GetValues(typeof(EnumServiceState)).Cast<EnumServiceState>().Select(v => new
				{
					Id = (int)v,
					Description = EnumHelper.GetDescriptionFromEnumValue(v)
				}).ToList();

				if (ret != null)
				{
					return Json(new { success = true, lista = ret }, JsonRequestBehavior.AllowGet);
				}
				else
					return Json(new { success = false, type = ValidateType.alert.ToString(), message = "Opções de situação não encontradas." }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				LogFacade.SaveError(ex);
				return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar opções de situação." }, JsonRequestBehavior.AllowGet);
			}
		}

		[HttpGet]
		[Paginate]
		public JsonResult SearchLotFiles(AnswerSheetLotFilter filter)
		{
			try
			{
				Pager pager = this.GetPager();
				var ret = answerSheetLotBusiness.GetLotFiles(filter, ref pager);

				if (ret != null && ret.Count() > 0)
				{
					var result = ret.Select(entity => new
					{
						Local = entity.SchoolName,
						DRE = entity.SupAdmUnitName,
						FileId = entity.FileId,
						FilePath = entity.FilePath
					});

					return Json(new { success = true, lista = result }, JsonRequestBehavior.AllowGet);
				}
				else
				{
					return Json(new { success = true, lista = "" }, JsonRequestBehavior.AllowGet);
				}
			}
			catch (Exception ex)
			{
				LogFacade.SaveError(ex);
				return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar os arquivos." }, JsonRequestBehavior.AllowGet);
			}
		}

		[HttpGet]
		[Paginate]
		public JsonResult SearchAdheredTests(AnswerSheetLotFilter filter)
		{
			try
			{
				Pager pager = this.GetPager();
				var ret = answerSheetLotBusiness.GetAdheredTests(filter, ref pager);

				if (ret != null && ret.Count() > 0)
				{
					var result = ret.Select(entity => new
					{
						Code = entity.TestCode,
						Description = entity.Description,
						TestType = entity.TestTypeDescription,
						CreateDate = entity.CreateDate.ToShortDateString(),
						TotalAdherence = entity.AllAdhered ? "Todas" : entity.TotalAdherence.ToString()
					});

					return Json(new { success = true, lista = result }, JsonRequestBehavior.AllowGet);
				}
				else
				{
					return Json(new { success = true, lista = "" }, JsonRequestBehavior.AllowGet);
				}
			}
			catch (Exception ex)
			{
				LogFacade.SaveError(ex);
				return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar as provas." }, JsonRequestBehavior.AllowGet);
			}
		}

		[HttpGet]
		public JsonResult SearchLotHistory(long Id)
		{
			try
			{
				AnswerSheetLot entity = answerSheetLotBusiness.GetById(Id);
				AnswerSheetLotHistory history = answerSheetLotBusiness.GetLotFolderSize(entity.Id, SessionFacade.UsuarioLogado.Usuario);
				history.Owner = entity.ExecutionOwner;

				return Json(new { success = true, lista = "", entity = history }, JsonRequestBehavior.AllowGet);

			}
			catch (Exception ex)
			{
				LogFacade.SaveError(ex);
				return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar o histórico." }, JsonRequestBehavior.AllowGet);
			}
		}

        #endregion

        #region AnswerSheetStudent

        [HttpGet]
        public async System.Threading.Tasks.Task<JsonResult> GetStudentBySection(long team_id, long test_id)
        {
            try
            {
                var lista = testBusiness.GetByTestSection(test_id, team_id);

                if (lista != null && lista.Count() > 0)
                {
                    List<SectionTestGenerateLot> sectionTestGenerateLot = (await sectionTestGenerateLotBusiness.Load(test_id, team_id)).ToList().FindAll(p => p.State != (byte)EnumState.excluido);

                    var ret = lista.Select(i => new
                    {
                        alu_id = i.alu_id,
                        alu_nome = i.alu_nome,
                        mtu_numeroChamada = i.mtu_numeroChamada,
                        blocked = i.blocked,
                        fileGenerateLot = sectionTestGenerateLot.Exists(p => p.alu_ids.Any(q => q == i.alu_id)),
                        FileAnswerSheet = new
                        {
                            Id = i.FileId,
                            Name = !string.IsNullOrEmpty(i.FileOriginalName) ? i.FileOriginalName : i.FileName,
                            Path = i.FilePath
                        }
                    });

                    return Json(new { success = true, lista = ret }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { success = true, lista = "" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar os alunos da turma." }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region Queue ZIP

        [HttpGet]
        [Paginate]
        public JsonResult GetUploadQueueStatus(EnumBatchQueueSituation? Situation)
        {

            try
            {
                Pager pager = this.GetPager();
                AnswerSheetBatchQueueFilter filter = new AnswerSheetBatchQueueFilter();

                filter.UserId = SessionFacade.UsuarioLogado.Usuario.usu_id;
                filter.PesId = SessionFacade.UsuarioLogado.Usuario.pes_id;
                filter.VisionId = SessionFacade.UsuarioLogado.Grupo.vis_id;
                filter.SystemId = Constants.IdSistema;
                filter.GroupId = SessionFacade.UsuarioLogado.Grupo.gru_id;
                filter.Situation = Situation;
                IEnumerable<AnswerSheetBatchQueueResult> answerSheetBatchQueue = answerSheetBatchQueueBusiness.Search(filter, ref pager);
                return Json(new { success = true, lista = answerSheetBatchQueue }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao pesquisar arquivos na fila de descompactação." }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        [Paginate]
        public JsonResult GetUploadQueueStatusDreSchool(EnumBatchQueueSituation? Situation, Guid? SupAdmUnitId, int? SchoolId, DateTime? StartDate, DateTime? EndDate, int? top)
        {

			try
			{
				Pager pager = this.GetPager();
				AnswerSheetBatchQueueFilter filter = new AnswerSheetBatchQueueFilter();

                filter.UserId = SessionFacade.UsuarioLogado.Usuario.usu_id;
                filter.PesId = SessionFacade.UsuarioLogado.Usuario.pes_id;
                filter.VisionId = SessionFacade.UsuarioLogado.Grupo.vis_id;
                filter.SystemId = Constants.IdSistema;
                filter.GroupId = SessionFacade.UsuarioLogado.Grupo.gru_id;
                filter.Situation = Situation;
                filter.SupAdmUnitId = SupAdmUnitId;
                filter.SchoolId = SchoolId;
                filter.StartDate = StartDate;
                filter.EndDate = EndDate;
                filter.top = top;

                IEnumerable<AnswerSheetBatchQueueResult> answerSheetBatchQueue = answerSheetBatchQueueBusiness.Search(filter, ref pager);

                if (answerSheetBatchQueue.Count() > 0)
                {
                    var ret = answerSheetBatchQueue.Select(v => new
                    {
                        Id = v.Id,
                        File_Id = v.File_Id,
                        FileName = v.FileName,
                        FilePath = v.FilePath,
                        AnswerSheetBatch_Id = v.AnswerSheetBatch_Id,
                        SupAdmUnit_Id = v.SupAdmUnit_Id,
                        SupAdmUnitName = v.SupAdmUnitName,
                        School_Id = v.School_Id,
                        SchoolName = v.SchoolName,
                        CountFiles = v.CountFiles,
                        Situation = v.Situation,
                        Description = v.Description,
                        CreatedBy_Id = v.CreatedBy_Id,
                        CreateDate = v.CreateDate,
                        UpdateDate = v.UpdateDate,
                        UserName = v.UserName,
                        Pending = v.Pending == null ? 0 : v.Pending,
                        Success = v.Success == null ? 0 : v.Success,
                        Error = v.Error == null ? 0 : v.Error,
                        Warning = v.Warning == null ? 0 : v.Warning,
                        PendingIdentification = v.PendingIdentification == null ? 0 : v.PendingIdentification,
                        NotIdentified = v.NotIdentified == null ? 0 : v.NotIdentified,
                        Absent = v.Absent == null ? 0 : v.Absent
                    }).ToList();

                    return Json(new { success = true, lista = ret }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { success = true, lista = answerSheetBatchQueue }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao pesquisar arquivos na fila de descompactação." }, JsonRequestBehavior.AllowGet);
            }
        }

		[HttpGet]
		public JsonResult GetUploadQueueTop(long Top)
		{

			try
			{
				AnswerSheetBatchQueueFilter filter = new AnswerSheetBatchQueueFilter();

				filter.UserId = SessionFacade.UsuarioLogado.Usuario.usu_id;
				filter.PesId = SessionFacade.UsuarioLogado.Usuario.pes_id;
				filter.VisionId = SessionFacade.UsuarioLogado.Grupo.vis_id;
				filter.SystemId = Constants.IdSistema;
				filter.GroupId = SessionFacade.UsuarioLogado.Grupo.gru_id;
				filter.top = Top;

				IEnumerable<AnswerSheetBatchQueueResult> answerSheetBatchQueue = answerSheetBatchQueueBusiness.GetTop(filter);
				return Json(new { success = true, lista = answerSheetBatchQueue }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				LogFacade.SaveError(ex);
				return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao pesquisar arquivos na fila de descompactação." }, JsonRequestBehavior.AllowGet);
			}
		}

        [HttpGet]
        public JsonResult GetSituationLot()
        {
            try
            {
                var listSituation = new List<EnumBatchQueueSituation>();

                listSituation.Add(EnumBatchQueueSituation.PendingUnzip);
                listSituation.Add(EnumBatchQueueSituation.Success);
                listSituation.Add(EnumBatchQueueSituation.NotUnziped);
                listSituation.Add(EnumBatchQueueSituation.Processing);

                var ret = listSituation.Select(v => new
                {
                    Id = (int)v,
                    Description = EnumHelper.GetDescriptionFromEnumValue(v)
                }).ToList();

                if (ret != null)
                {
                    return Json(new { success = true, lista = ret }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { success = false, type = ValidateType.alert.ToString(), message = "Situações do lote não encontradas." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar situações do lote." }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

		#endregion

		#region Write

		[HttpPost]
		public JsonResult SaveBatch(AnswerSheetBatch entity)
		{
			try
			{
				entity = batchFilesBusiness.SaveBatch(entity, ApplicationFacade.VirtualDirectory, ApplicationFacade.PhysicalDirectory, SessionFacade.UsuarioLogado.Usuario);
			}
			catch (Exception ex)
			{
				entity.Validate.IsValid = false;
				entity.Validate.Type = ValidateType.error.ToString();
				entity.Validate.Message = string.Format("Erro ao {0} o lote de folhas de respostas.", entity.Id > 0 ? "alterar" : "salvar");

				LogFacade.SaveError(ex);
			}

			return Json(new { success = entity.Validate.IsValid, type = entity.Validate.Type, message = entity.Validate.Message, batchId = entity.Id }, JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		public JsonResult GenerateAnswerSheet(long Id, long teamId, int schoolId, int type, long studentId)
		{
			Booklet booklet = bookletBusiness.GetBookletByTest(Id);
			object ret = null;
			List<AnswerSheetStudentInformation> students = null;
			EntityFile entity = new EntityFile();
			string virtualDirectory = ApplicationFacade.VirtualDirectory;
			string physicalDirectory = ApplicationFacade.PhysicalDirectory;
			string folderName = "QRCodes";

			try
			{
				if (booklet != null)
				{
					if (teamId > 0 && schoolId <= 0)
					{
						TUR_Turma turma = turmaBusiness.Get(teamId);
						ESC_Escola escola = escolaBusiness.Get(turma.esc_id);
						schoolId = escola.esc_id;
					}

					#region Validation

					if (type.Equals((int)EnumIdentificationType.QRCode))
					{
						if (schoolId <= 0)
						{
							entity.Validate.IsValid = false;
							entity.Validate.Message = "É necessário selecionar uma escola para gerar o PDF.";
							entity.Validate.Type = ValidateType.alert.ToString();
						}
						else if (teamId <= 0)
						{
							entity.Validate.IsValid = false;
							entity.Validate.Message = "É necessário selecionar uma turma para gerar o PDF.";
							entity.Validate.Type = ValidateType.alert.ToString();
						}
					}

					#endregion

					if (entity.Validate.IsValid)
					{
						#region QRCode

						if (type.Equals((int)EnumIdentificationType.QRCode))
						{
							students = testBusiness.GetTeamStudents(schoolId, teamId, studentId, Id, booklet.Test.AllAdhered).ToList();

							if (students != null && students.Count > 0)
							{
								foreach (AnswerSheetStudentInformation student in students)
								{
									string name = Guid.NewGuid() + ".png";
									string QRCodeId = "{";
									QRCodeId += string.Format(@" ""Test_Id"": {0}, ""School_Id"": {1}, ""Section_Id"": {2}, ""Student_Id"": {3} ", Id, schoolId, teamId, student.Id);
									QRCodeId += "}";

									student.RelativePath = answerSheetBusiness.RenderQrCode(QRCodeId, 5, virtualDirectory, physicalDirectory, name, folderName);
									if (!string.IsNullOrEmpty(student.RelativePath))
										student.StoragePath = string.Concat(physicalDirectory, new Uri(student.RelativePath).AbsolutePath.Replace("Files/", string.Empty).Replace("/", "\\"));
									else
									{
										entity.Validate.Type = ValidateType.error.ToString();
										entity.Validate.IsValid = false;
										entity.Validate.Message = "Erro ao gerar QRCode do pdf da(s) folha(s) de respostas.";
										break;
									}
								}
							}
						}

						#endregion

						if (entity.Validate.IsValid)
						{
							#region PDF

							PDFFilter filter = new PDFFilter
							{
								Booklet = booklet,
								Test = booklet.Test,
								FontSize = 20,
								UrlSite = Request.Url.Authority.ToString(),
								ContentType = "application/pdf",
								VirtualDirectory = virtualDirectory,
								PhysicalDirectory = physicalDirectory,
								GenerateType = EnumGenerateType.AnswerSheet,
								FileType = type.Equals((int)EnumIdentificationType.QRCode) ? EnumFileType.AnswerSheetQRCode : EnumFileType.AnswerSheetStudentNumber,
								OwnerId = studentId > 0 ? studentId : teamId,
								ParentOwnerId = booklet.Test.Id,
								StudentList = students,
								IdentificationType = type.Equals((int)EnumIdentificationType.QRCode) ? EnumIdentificationType.QRCode : type.Equals((int)EnumIdentificationType.NumberID) ? EnumIdentificationType.NumberID : EnumIdentificationType.OnlyWrite
							};

							entity = bookletBusiness.SavePdfTest(filter, SessionFacade.UsuarioLogado.Usuario.ent_id);

							if (entity.Validate.IsValid)
							{
								entity.Validate.IsValid = true;
								entity.Validate.Message = "PDF da(s) folha(s) de respostas gerado com sucesso.";

								ret = new
								{
									TestId = booklet.Test.Id,
									FileAnswerSheet = entity != null ? new
									{
										Id = entity.Id,
										Name = !string.IsNullOrEmpty(entity.OriginalName) ? entity.OriginalName : entity.Name,
										Path = entity.Path
									} : null
								};
							}

							#endregion
						}
					}
				}
				else
				{
					entity.Validate.Type = ValidateType.alert.ToString();
					entity.Validate.IsValid = false;
					entity.Validate.Message = "Não existe o caderno da prova selecionada.";
				}
			}
			catch (Exception ex)
			{
				entity.Validate.Type = ValidateType.error.ToString();
				entity.Validate.IsValid = false;
				entity.Validate.Message = "Erro ao gerar pdf da(s) folha(s) de respostas.";

				LogFacade.SaveError(ex);
			}

			#region QRCode

			try
			{
				if (type.Equals((int)EnumIdentificationType.QRCode))
				{
					//Exclui as imagens QRCode após o processo de gerar PDF
					if (students != null && students.Count > 0)
					{
						foreach (AnswerSheetStudentInformation student in students)
						{
							if (!string.IsNullOrEmpty(student.StoragePath) && System.IO.File.Exists(student.StoragePath))
								System.IO.File.Delete(student.StoragePath);
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogFacade.SaveError(ex);
			}

			#endregion

			return Json(new { success = entity.Validate.IsValid, type = entity.Validate.Type, message = entity.Validate.Message, generateTest = ret }, JsonRequestBehavior.AllowGet);
		}

        [HttpPost]
        public JsonResult DeleteBatchQueueAndFiles(long Id)
        {
            AnswerSheetBatchQueue entity = new AnswerSheetBatchQueue();

            try
            {
                entity = answerSheetBatchQueueBusiness.Delete(Id, SessionFacade.UsuarioLogado.Usuario.ent_id);
            }
            catch (Exception ex)
            {
                entity.Validate.IsValid = false;
                entity.Validate.Type = ValidateType.error.ToString();
                entity.Validate.Message = "Erro ao tentar excluir o arquivo zip.";
                LogFacade.SaveError(ex);
            }

            return Json(new { success = entity.Validate.IsValid, type = entity.Validate.Type, message = entity.Validate.Message }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteBatchFilesError(long Id)
        {
            AnswerSheetBatchQueue entity = new AnswerSheetBatchQueue();

            try
            {
                entity = answerSheetBatchQueueBusiness.DeleteError(Id, SessionFacade.UsuarioLogado.Usuario.ent_id);
            }
            catch (Exception ex)
            {
                entity.Validate.IsValid = false;
                entity.Validate.Type = ValidateType.error.ToString();
                entity.Validate.Message = "Erro ao tentar excluir os arquivos com erro.";
                LogFacade.SaveError(ex);
            }

            return Json(new { success = entity.Validate.IsValid, type = entity.Validate.Type, message = entity.Validate.Message }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteFileById(AnswerSheetBatchFiles file)
        {
            AnswerSheetBatchFiles entity = new AnswerSheetBatchFiles();

            try
            {
                List<AnswerSheetBatchFiles> list = new List<AnswerSheetBatchFiles>();
                list.Add(file);
                batchFilesBusiness.DeleteList(list);
                entity.Validate.IsValid = true;
                entity.Validate.Message = "Arquivo deletado com sucesso.";
            }
            catch (Exception ex)
            {
                entity.Validate.IsValid = false;
                entity.Validate.Type = ValidateType.error.ToString();
                entity.Validate.Message = "Erro ao tentar excluir o arquivo.";
                LogFacade.SaveError(ex);
            }

            return Json(new { success = entity.Validate.IsValid, type = entity.Validate.Type, message = entity.Validate.Message }, JsonRequestBehavior.AllowGet);
        }

        #region AnswerSheetLot

		[HttpPost]
		public JsonResult SaveLot(AnswerSheetLot entity, List<AnswerSheetLot> list)
		{
			try
			{
				entity = answerSheetLotBusiness.Save(entity, list);
			}
			catch (Exception ex)
			{
				entity.Validate.IsValid = false;
				entity.Validate.Type = ValidateType.error.ToString();
				entity.Validate.Message = string.Format("Erro ao {0} lote de folha de resposta.", entity.StateExecution.Equals(EnumServiceState.NotRequest) || entity.StateExecution.Equals(EnumServiceState.Pending) ? "solicitar" : "desfazer a solicitação do");

				LogFacade.SaveError(ex);
			}

			return Json(new { success = entity.Validate.IsValid, type = entity.Validate.Type, message = entity.Validate.Message }, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		public JsonResult GenerateLotAgain(AnswerSheetLot entity)
		{
			try
			{
				entity = answerSheetLotBusiness.GenerateAgain(entity, SessionFacade.UsuarioLogado.Usuario.usu_id);
			}
			catch (Exception ex)
			{
				entity.Validate.IsValid = false;
				entity.Validate.Type = ValidateType.error.ToString();
				entity.Validate.Message = "Erro ao gerar novamente o lote de folha de resposta.";

				LogFacade.SaveError(ex);
			}

			return Json(new { success = entity.Validate.IsValid, type = entity.Validate.Type, message = entity.Validate.Message }, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		public JsonResult DeleteLot(AnswerSheetLot entity)
		{
			try
			{
				entity = answerSheetLotBusiness.Delete(entity);
			}
			catch (Exception ex)
			{
				entity.Validate.IsValid = false;
				entity.Validate.Type = ValidateType.error.ToString();
				entity.Validate.Message = "Erro ao tentar excluir o lote de folha de resposta.";
				LogFacade.SaveError(ex);
			}

			return Json(new { success = entity.Validate.IsValid, type = entity.Validate.Type, message = entity.Validate.Message }, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#endregion

		#region Private Methods

		private AnswerSheetBatchFilter GetFilter(AnswerSheetBatchFilter filter)
		{
			if (filter == null)
				filter = new AnswerSheetBatchFilter();

			filter.UserId = SessionFacade.UsuarioLogado.Usuario.usu_id;
			filter.PesId = SessionFacade.UsuarioLogado.Usuario.pes_id;
			filter.CoreVisionId = SessionFacade.UsuarioLogado.Grupo.vis_id;
			filter.CoreSystemId = Constants.IdSistema;
			filter.EntId = SessionFacade.UsuarioLogado.Usuario.ent_id;
			filter.CoreGroupId = SessionFacade.UsuarioLogado.Grupo.gru_id;

			if (filter.BatchId > 0)
			{
				AnswerSheetBatch batch = batchBusiness.Get(filter.BatchId);
				if (batch != null)
				{
					if (filter.TestId <= 0)
						filter.TestId = batch.Test_Id;

					if (filter.SchoolId <= 0)
						filter.SchoolId = batch.School_Id != null ? batch.School_Id : 0;

					if (filter.SectionId <= 0)
						filter.SectionId = batch.Section_Id != null ? batch.Section_Id : 0;
				}
			}

			return filter;
		}

		#endregion
	}
}