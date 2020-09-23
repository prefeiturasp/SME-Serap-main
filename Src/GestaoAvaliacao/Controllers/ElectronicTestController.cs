using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Entities.DTO;
using GestaoAvaliacao.Entities.DTO.Tests;
using GestaoAvaliacao.Entities.Enumerator;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.MongoEntities;
using GestaoAvaliacao.Util;
using GestaoAvaliacao.WebProject.Facade;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace GestaoAvaliacao.Controllers
{
    public class ElectronicTestController : Controller
    {
        private readonly ITestBusiness testBusiness;
        private readonly IAlternativeBusiness alternativeBusiness;
        private readonly IStudentCorrectionBusiness _studentCorrectionBusiness;
        private readonly ICorrectionBusiness correctionBusiness;
        private readonly IItemFileBusiness itemFileBusiness;
        private readonly IItemAudioBusiness itemAudioBusiness;

        public ElectronicTestController(ITestBusiness testBusiness, IAlternativeBusiness alternativeBusiness, IStudentCorrectionBusiness _studentCorrectionBusiness, ICorrectionBusiness correctionBusiness,
            IItemFileBusiness itemFileBusiness, IItemAudioBusiness itemAudioBusiness)
        {
            this.testBusiness = testBusiness;
            this.alternativeBusiness = alternativeBusiness;
            this._studentCorrectionBusiness = _studentCorrectionBusiness;
            this.correctionBusiness = correctionBusiness;
            this.itemFileBusiness = itemFileBusiness;
            this.itemAudioBusiness = itemAudioBusiness;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Form(long? TestId, long? AluId, long? TurId)
        {
            if (TestId.HasValue && AluId.HasValue && TurId.HasValue)
            {

                if (VerifyPermission(TestId, AluId, TurId))
                {
                    return View();
                }
                else
                {
                    return RedirectToAction("index", "home");
                }
            }
            else
            {
                return RedirectToAction("index", "home");
            }
        }

        private bool VerifyPermission(long? TestId, long? AluId, long? TurId)
        {
            bool isValid = true;

            if ((EnumSYS_Visao)SessionFacade.UsuarioLogado.Grupo.vis_id == EnumSYS_Visao.Individual)
            {
                if (AluId != SessionFacade.UsuarioLogado.alu_id)
                {
                    return false;
                }

                if (!testBusiness.ExistsAdherenceByAluIdTestId(AluId.Value, TestId.Value))
                {
                    return false;
                }
            }

            return isValid;
        }

        [HttpGet]
        public JsonResult Load()
        {
            try
            {
                var vis_id = (EnumSYS_Visao)Enum.Parse(typeof(EnumSYS_Visao), SessionFacade.UsuarioLogado.Grupo.vis_id.ToString());

                List<ElectronicTestDTO> lista = new List<ElectronicTestDTO>();

                if (vis_id != EnumSYS_Visao.Individual)
                {
                    lista = testBusiness.SearchEletronicTests();

                    if (lista != null)
                    {
                        var ret = lista.Select(entity => new ElectronicTestDTO
                        {
                            Id = entity.Id,
                            Description = entity.Description,
                            NumberItem = entity.NumberItem,
                            quantDiasRestantes = entity.quantDiasRestantes,
                            FrequencyApplicationText = EnumHelper.GetDescriptionFromEnumValue((EnumFrenquencyApplication)entity.FrequencyApplication),
                            ApplicationEndDate = entity.ApplicationEndDate
                        }).ToList();

                        List<ElectronicTestDTO> listaNaoIniciada = new List<ElectronicTestDTO>();
                        List<ElectronicTestDTO> listaEmAndamento = new List<ElectronicTestDTO>();
                        List<ElectronicTestDTO> listaFinalizadas = new List<ElectronicTestDTO>();

                        listaEmAndamento = ret.Where(p => p.quantDiasRestantes > 0).ToList();

                        listaFinalizadas = ret.Where(p => p.quantDiasRestantes == 0).ToList();

                        return Json(new { success = true, listaNaoIniciada = listaNaoIniciada, listaEmAndamento = listaEmAndamento, listaFinalizadas = listaFinalizadas }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    lista = testBusiness.SearchEletronicTestsByPesId(SessionFacade.UsuarioLogado.Usuario.pes_id);

                    if (lista != null)
                    {
                        var ret = lista.Select(entity => new ElectronicTestDTO
                        {
                            Id = entity.Id,
                            Description = entity.Description,
                            NumberItem = entity.NumberItem,
                            quantDiasRestantes = entity.quantDiasRestantes,
                            FrequencyApplicationText = EnumHelper.GetDescriptionFromEnumValue((EnumFrenquencyApplication)entity.FrequencyApplication),
                            ApplicationEndDate = entity.ApplicationEndDate,
                            alu_id = entity.alu_id,
                            tur_id = entity.tur_id
                        }).ToList();

                        List<ElectronicTestDTO> listaNaoIniciada = new List<ElectronicTestDTO>();
                        List<ElectronicTestDTO> listaEmAndamento = new List<ElectronicTestDTO>();
                        List<ElectronicTestDTO> listaFinalizadas = new List<ElectronicTestDTO>();

                        foreach (var item in ret)
                        {
                            StudentCorrection studentCorrection = _studentCorrectionBusiness.GetStudentCorrectionByTestAluId(item.Id, item.alu_id, item.tur_id);

                            if (studentCorrection != null && !studentCorrection.provaFinalizada.HasValue)
                            {
                                studentCorrection.provaFinalizada = false;
                            }

                            if (studentCorrection == null && item.quantDiasRestantes > 0)
                            {
                                listaNaoIniciada.Add(item);
                            }
                            else if (studentCorrection != null && !studentCorrection.provaFinalizada.Value && item.quantDiasRestantes > 0)
                            {
                                listaEmAndamento.Add(item);
                            }
                            else
                            {
                                listaFinalizadas.Add(item);
                            }
                        }

                        return Json(new { success = true, listaNaoIniciada = listaNaoIniciada, listaEmAndamento = listaEmAndamento, listaFinalizadas = listaFinalizadas }, JsonRequestBehavior.AllowGet);
                    }
                }

                return Json(new { success = false, type = ValidateType.alert.ToString(), message = "Provas não encontradas." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar as provas." }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public async Task<JsonResult> LoadByTestId(long test_id, long alu_id, long tur_id)
        {
            try
            {
                var testTask = testBusiness.SearchInfoTestAsync(test_id);
                var itensTask = LoadItensAsync(test_id);
                var studentCorrectionTask = _studentCorrectionBusiness.Get(alu_id, test_id, tur_id, SessionFacade.UsuarioLogado.Usuario.ent_id);
                await Task.WhenAll(testTask, itensTask, studentCorrectionTask);

                if (testTask.Result is null  || itensTask.Result is null)
                {
                    return Json(new { success = false, type = ValidateType.alert.ToString(), message = "Dados não encontrados." }, JsonRequestBehavior.AllowGet);
                }

                await LoadAlternativesByItensAsync(test_id, itensTask.Result);

                var ret = new TestModelDto
                {
                    Id = testTask.Result.Id,
                    Description = testTask.Result.Description,
                    NumberItem = testTask.Result.NumberItem,
                    QuantDiasRestantes = testTask.Result.quantDiasRestantes,
                    FrequencyApplication = EnumHelper.GetDescriptionFromEnumValue((EnumFrenquencyApplication)testTask.Result.FrequencyApplication),
                    ApplicationEndDate = testTask.Result.ApplicationEndDate.ToString("dd/MM/yyyy"),
                    LastAnswer = studentCorrectionTask.Result?.OrdemUltimaResposta,
                    Done = studentCorrectionTask.Result?.provaFinalizada ?? false,
                    Itens = itensTask.Result
                };

                return Json(new { success = true, test = ret }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar os dados da prova." }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public async Task<JsonResult> LoadAnswersAsync(long test_id, long alu_id, long tur_id)
        {
            try
            {
                var studentCorrection = await _studentCorrectionBusiness.Get(alu_id, test_id, tur_id, SessionFacade.UsuarioLogado.Usuario.ent_id);
                if(studentCorrection is null)
                    return Json(new { success = true, answers = new List<AnswerModelDto>() }, JsonRequestBehavior.AllowGet);

                var answers = studentCorrection.Answers
                    .Select(x => new AnswerModelDto
                    {
                        AlternativeId = x.AnswerChoice,
                        Changed = false,
                        ItemId = x.Item_Id
                    })
                    .ToList();

                return Json(new { success = true, answers }, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                LogFacade.SaveError(ex);
                return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar os dados da prova." }, JsonRequestBehavior.AllowGet);
            }
        }

        private async Task<IEnumerable<ItemModelDto>> LoadItensAsync(long test_id)
        {
            var blockItems = await testBusiness.GetItemsByTestAsync(test_id, SessionFacade.UsuarioLogado.Usuario.usu_id);
            var lstItens = blockItems.Select(x => x.Item.Id).Distinct().ToList();

            var testShowVideoAudioFilesDto = testBusiness.GetTestShowVideoAudioFiles(test_id);
            var itemVideos = testShowVideoAudioFilesDto.ShowVideoFiles ? itemFileBusiness.GetVideosByLstItemId(lstItens) : new List<ItemFile>();
            var itemAudios = testShowVideoAudioFilesDto.ShowAudioFiles ? itemAudioBusiness.GetAudiosByLstItemId(lstItens) : new List<ItemAudio>();

            return blockItems.Select(bi =>
            {
                var item = new ItemModelDto
                {
                    Id = bi.Item.Id,
                    ItemCode = bi.Item.ItemCode,
                    ItemVersion = bi.Item.ItemVersion,
                    ItemOrder = bi.Order,
                    Statement = bi.Item.Statement,
                    Revoked = EnumSituation.NotRevoked,
                    ItemSituation = EnumSituation.NotRevoked,
                    BlockItem_Id = bi.Id,
                    Videos = itemVideos.Where(p => p.Item_Id == bi.Item.Id),
                    Audios = itemAudios.Where(p => p.Item_Id == bi.Item.Id),
                    ShowVideoFiles = testShowVideoAudioFilesDto.ShowVideoFiles && itemVideos.Any(p => p.Item_Id == bi.Item.Id),
                    ShowAudioFiles = testShowVideoAudioFilesDto.ShowAudioFiles && itemAudios.Any(p => p.Item_Id == bi.Item.Id)
                };

                var requestRevoke = bi.RequestRevokes?.FirstOrDefault();
                if(requestRevoke != null)
                {
                    item.Revoked = requestRevoke.Situation;
                    item.ItemSituation = requestRevoke.Situation;
                    item.RequestRevoke_Id = requestRevoke.Id;
                    item.Justification = requestRevoke.Justification;
                }

                if(bi.Item.BaseText != null)
                {
                    item.BaseTextDescription = bi.Item.BaseText.Description;
                    item.BaseTextId = bi.Item.BaseText.Id;
                }

                return item;
            }).ToList();
        }

        private async Task LoadAlternativesByItensAsync(long test_id, IEnumerable<ItemModelDto> itens)
        {
            var itensIdsConcat = itens.Select(x => string.Concat("'", x.Id, "'"));

            var alternatives = await alternativeBusiness.GetAlternativesByItensAsync(itensIdsConcat, test_id);
            if (!alternatives?.Any() ?? true) throw new NullReferenceException("Não foram encontradas alternativas para os itens da prova.");

            var alternativesModel = alternatives
                .Select(x => new AlternativeModelDto
                {
                    Id = x.Id,
                    Description = x.Description,
                    Item_Id = x.Item_Id,
                    Justificative = x.Justificative,
                    Numeration = x.Numeration,
                    NumerationSem = x.NumerationSem,
                    Order = x.Order,
                    Selected = false
                })
                .ToList();

            foreach(var item in itens)
            {
                var artenativesOfItem = alternativesModel.Where(x => x.Item_Id == item.Id).ToList();
                item.Alternatives.AddRange(artenativesOfItem);
            }
        }

        [HttpPost]
        public async Task<JsonResult> Save(Alternative alternativa, long test_id, long alu_id, long tur_id, int ordemItem)
        {
            try
            {
                Guid ent_id = SessionFacade.UsuarioLogado.Usuario.ent_id;
                Guid usu_id = SessionFacade.UsuarioLogado.Usuario.usu_id;
                Guid pes_id = SessionFacade.UsuarioLogado.Usuario.pes_id;
                EnumSYS_Visao vis_id = (EnumSYS_Visao)SessionFacade.UsuarioLogado.Grupo.vis_id;

                await correctionBusiness.SaveCorrection(alu_id, alternativa.Id, alternativa.Item_Id, false, false, test_id, tur_id, ent_id,
                   usu_id, pes_id, vis_id, true, false, ordemItem, true);

                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar a área de conhecimento pesquisada." }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public async Task<JsonResult> SaveAnswersAsync(IEnumerable<AnswerModelDto> answers, long test_id, long alu_id, long tur_id, int ordemItem)
        {
            try
            {
                var ent_id = SessionFacade.UsuarioLogado.Usuario.ent_id;
                var usu_id = SessionFacade.UsuarioLogado.Usuario.usu_id;
                var pes_id = SessionFacade.UsuarioLogado.Usuario.pes_id;
                var vis_id = (EnumSYS_Visao)SessionFacade.UsuarioLogado.Grupo.vis_id;

                await correctionBusiness.SaveCorrectionAsync(test_id, alu_id, tur_id, answers, ent_id, usu_id, pes_id, vis_id, ordemItem);
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                LogFacade.SaveError(ex);
                return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar salvar as resposta da prova." }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public async Task<JsonResult> FinalizeCorrection(long tur_id, long test_id, long alu_id)
        {
            Adherence entity = new Adherence();
            try
            {
                entity = await correctionBusiness.FinalizeCorrectionElectronicTest(tur_id, test_id, alu_id, SessionFacade.UsuarioLogado.Usuario, (EnumSYS_Visao)SessionFacade.UsuarioLogado.Grupo.vis_id);
                return Json(new { success = true, message = "Prova entregue com sucesso." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                entity.Validate.IsValid = false;
                entity.Validate.Type = ValidateType.error.ToString();
                entity.Validate.Message = "Erro ao entregar prova.";

                LogFacade.SaveError(ex);
            }
            return Json(new { success = entity.Validate.IsValid, type = entity.Validate.Type, message = entity.Validate.Message }, JsonRequestBehavior.AllowGet);
        }
    }
}