using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Entities.DTO;
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
        public JsonResult LoadByTestId(long test_id)
        {
            try
            {
                var test = testBusiness.SearchInfoTest(test_id);

                var ret = new
                {
                    Id = test.Id,
                    Description = test.Description,
                    NumberItem = test.NumberItem,
                    quantDiasRestantes = test.quantDiasRestantes,
                    FrequencyApplication = EnumHelper.GetDescriptionFromEnumValue((EnumFrenquencyApplication)test.FrequencyApplication),
                    ApplicationEndDate = test.ApplicationEndDate.ToString("dd/MM/yyyy")
                };

                if (test != null)
                {
                    return Json(new { success = true, test = ret }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { success = false, type = ValidateType.alert.ToString(), message = "Dados não encontrados." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar os dados da prova." }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult LoadItensByTestId(long test_id)
        {
            try
            {
                var blockItems = testBusiness.GetItemsByTest(test_id, SessionFacade.UsuarioLogado.Usuario.usu_id);

                List<long> lstItens = blockItems.Select(x => x.Item.Id).Distinct().ToList();

                var itemVideos = itemFileBusiness.GetVideosByLstItemId(lstItens);
                var itemAudios = itemAudioBusiness.GetAudiosByLstItemId(lstItens);

                var retorno = blockItems.Select(bi => new
                {
                    Item_Id = bi.Item.Id,
                    ItemCode = bi.Item.ItemCode,
                    ItemVersion = bi.Item.ItemVersion,
                    ItemOrder = bi.Order,
                    BaseTextDescription = bi.Item.BaseText != null ? bi.Item.BaseText.Description : "",
                    BaseTextId = bi.Item.BaseText != null ? bi.Item.BaseText.Id : 0,
                    Statement = bi.Item.Statement != null ? bi.Item.Statement : "",
                    Revoked = bi.RequestRevokes != null ? bi.RequestRevokes.First().Situation : EnumSituation.NotRevoked,
                    ItemSituation = bi.RequestRevokes != null ? bi.RequestRevokes.First().Situation : EnumSituation.NotRevoked,
                    RequestRevoke_Id = bi.RequestRevokes != null ? bi.RequestRevokes.First().Id : 0,
                    BlockItem_Id = bi.Id,
                    Justification = bi.RequestRevokes != null ? bi.RequestRevokes.First().Justification : "",
                    Videos = itemVideos.Where(p => p.Item_Id == bi.Item.Id),
                    Audios = itemAudios.Where(p => p.Item_Id == bi.Item.Id)
                }).ToList();

                return Json(new { success = true, itens = retorno }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar os itens da prova." }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public async Task<JsonResult> LoadAlternativesByItens(long[] itens, long test_id, long alu_id, long tur_id)
        {
            try
            {
                var itensConcat = itens.Distinct().AsEnumerable().Select(x => string.Concat("'", x, "'"));

                var alternatives = alternativeBusiness.GetAlternativesByItens(itensConcat, test_id);

                bool provaFinalizada = false;

                if (alternatives != null)
                {
                    if (alu_id > 0 && tur_id > 0)
                    {
                        StudentCorrection studentCorrection = await _studentCorrectionBusiness.Get(alu_id, test_id, tur_id, SessionFacade.UsuarioLogado.Usuario.ent_id);

                        int ordemUltimaResposta = 0;

                        if (studentCorrection != null)
                        {
                            foreach (var alternativa in alternatives)
                            {
                                alternativa.Justificative = null;
                                alternativa.Correct = false;
                                alternativa.Selected = studentCorrection.Answers.Exists(p => p.Item_Id == alternativa.Item_Id && p.AnswerChoice == alternativa.Id);
                            }

                            ordemUltimaResposta = studentCorrection.OrdemUltimaResposta.Value;
                            provaFinalizada = studentCorrection.provaFinalizada.Value;
                        }
                        else
                        {
                            foreach (var alternativa in alternatives)
                            {
                                alternativa.Justificative = null;
                                alternativa.Correct = false;
                            }
                        }

                        return Json(new { success = true, alternatives = alternatives, ordemUltimaResposta = ordemUltimaResposta, provaFinalizada = provaFinalizada, existemDados = studentCorrection != null }, JsonRequestBehavior.AllowGet);
                    }
                    return Json(new { success = true, alternatives = alternatives, ordemUltimaResposta = 0, provaFinalizada = provaFinalizada, existemDados = false }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { success = false, type = ValidateType.alert.ToString(), message = "Dados não encontrados." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar a área de conhecimento pesquisada." }, JsonRequestBehavior.AllowGet);
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