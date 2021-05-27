using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Entities.DTO;
using GestaoAvaliacao.Entities.DTO.StudentsTestSent;
using GestaoAvaliacao.Entities.DTO.Tests;
using GestaoAvaliacao.Entities.Enumerator;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.Util;
using GestaoAvaliacao.WebProject.Facade;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using GestaoAvaliacao.Dtos.StudentTestAccoplishments;

namespace GestaoAvaliacao.Controllers
{
    public class ElectronicTestResultController : Controller
    {
        private readonly ITestBusiness testBusiness;
        private readonly ITestTimeBusiness testTimeBusiness;
        private readonly IAlternativeBusiness alternativeBusiness;
        private readonly IStudentCorrectionBusiness _studentCorrectionBusiness;
        private readonly ICorrectionBusiness correctionBusiness;
        private readonly IItemFileBusiness itemFileBusiness;
        private readonly IItemAudioBusiness itemAudioBusiness;
        private readonly IStudentTestAccoplishmentBusiness studentTestAccoplishmentBusiness;

        public ElectronicTestResultController(ITestBusiness testBusiness, ITestTimeBusiness testTimeBusiness, IAlternativeBusiness alternativeBusiness, IStudentCorrectionBusiness _studentCorrectionBusiness, ICorrectionBusiness correctionBusiness,
            IItemFileBusiness itemFileBusiness, IItemAudioBusiness itemAudioBusiness, IStudentTestAccoplishmentBusiness studentTestAccoplishmentBusiness)
        {
            this.testBusiness = testBusiness;
            this.testTimeBusiness = testTimeBusiness;
            this.alternativeBusiness = alternativeBusiness;
            this._studentCorrectionBusiness = _studentCorrectionBusiness;
            this.correctionBusiness = correctionBusiness;
            this.itemFileBusiness = itemFileBusiness;
            this.itemAudioBusiness = itemAudioBusiness;
            this.studentTestAccoplishmentBusiness = studentTestAccoplishmentBusiness;
        }

        public async Task<ActionResult> Index(int? Ano, long? TestId, long? AluId, long? TurId)
        {
            if (TestId.HasValue && AluId.HasValue && TurId.HasValue)
            {
                if (await VerifyPermission(TestId, AluId, TurId))
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

        private async Task<bool> VerifyPermission(long? TestId, long? AluId, long? TurId)
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

                var studentCorrection = await _studentCorrectionBusiness.GetStudentCorrectionByTestAluId(TestId.Value, AluId.Value, TurId.Value);
                if (studentCorrection == null || studentCorrection.provaFinalizada == null || !studentCorrection.provaFinalizada.Value)
                    return isValid;
                else 
                    return false;
            }

            return isValid;
        }

        [HttpGet]
        public async Task<JsonResult> Load()
        {
            try
            {
                var vis_id = (EnumSYS_Visao)Enum.Parse(typeof(EnumSYS_Visao), SessionFacade.UsuarioLogado.Grupo.vis_id.ToString());
                return vis_id == EnumSYS_Visao.Individual
                    ? await LoadByIndividualVisionAsync()
                    : await LoadByAdminVisionAsync();
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar as provas." }, JsonRequestBehavior.AllowGet);
            }
        }

        private async Task<JsonResult> LoadByAdminVisionAsync()
        {
            var electronicTests = await testBusiness.SearchEletronicTests();
            if (electronicTests is null)
                return Json(new { success = false, type = ValidateType.alert.ToString(), message = "Provas não encontradas." }, JsonRequestBehavior.AllowGet);

            var listaNaoIniciada = new List<ElectronicTestDTO>();
            var listaEmAndamento = electronicTests.Where(p => p.quantDiasRestantes > 0).ToList();
            var listaFinalizadas = electronicTests.Where(p => p.quantDiasRestantes == 0).ToList();

            return Json(new { success = true, listaNaoIniciada, listaEmAndamento, listaFinalizadas }, JsonRequestBehavior.AllowGet);
        }

        private async Task<JsonResult> LoadByIndividualVisionAsync()
        {
            var electronicTests = await testBusiness.SearchEletronicTestsByPesId(SessionFacade.UsuarioLogado.Usuario.pes_id);
            if (electronicTests is null)
                return Json(new { success = false, type = ValidateType.alert.ToString(), message = "Provas não encontradas." }, JsonRequestBehavior.AllowGet);

            var listaNaoIniciada = new List<ElectronicTestDTO>();
            var listaEmAndamento = new List<ElectronicTestDTO>();
            var listaFinalizadas = new List<ElectronicTestDTO>();
            var listaDeTemposDeProva = new List<StudentTestTimeDto>();

            if (electronicTests?.Any() ?? false)
                listaDeTemposDeProva = await studentTestAccoplishmentBusiness.GetAsyncByAluIdTestId(electronicTests.FirstOrDefault().alu_id,
                    electronicTests.Select(s => s.Id).ToList());

            foreach (var electronicTest in electronicTests)
            {
                var studentCorrection = await _studentCorrectionBusiness.GetStudentCorrectionByTestAluId(electronicTest.Id, electronicTest.alu_id, electronicTest.tur_id);
                if (studentCorrection != null && !studentCorrection.provaFinalizada.HasValue)
                {
                    studentCorrection.provaFinalizada = false;
                }

                var tempoDeProva = listaDeTemposDeProva.FirstOrDefault(f => f.TestId == electronicTest.Id);
                if (tempoDeProva != null)
                {
                    var time = TimeSpan.FromSeconds(tempoDeProva.TempoEmSegundosUsadoPeloEstudanteNaProva - tempoDeProva.TempoEmSegundosDaProva);
                    electronicTest.TempoDeDuracaoDaProva = tempoDeProva.TempoEmSegundosDaProva == 0
                        ? "Sem limite de tempo"
                        : tempoDeProva.TempoDeDuracaoDaProva;
                    electronicTest.TempoRestanteDeProva =
                        tempoDeProva.TempoEmSegundosDaProva > 0 && tempoDeProva.TempoEmSegundosUsadoPeloEstudanteNaProva < tempoDeProva.TempoEmSegundosDaProva
                            ? time.ToString(@"hh\:mm\:ss")
                            : "00:00:00";
                }

                if (studentCorrection == null && electronicTest.quantDiasRestantes > 0)
                {
                    listaNaoIniciada.Add(electronicTest);
                }
                else if (studentCorrection != null && !studentCorrection.provaFinalizada.Value && electronicTest.quantDiasRestantes > 0)
                {
                    listaEmAndamento.Add(electronicTest);
                }
                else
                {
                    listaFinalizadas.Add(electronicTest);
                }
            }

            return Json(new { success = true, listaNaoIniciada, listaEmAndamento, listaFinalizadas }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<JsonResult> LoadByTestId(long test_id)
        {
            try
            {
                var test = await testBusiness.SearchInfoTestAsync(test_id);
                if (test is null)
                {
                    return Json(new { success = false, type = ValidateType.alert.ToString(), message = "Dados não encontrados." }, JsonRequestBehavior.AllowGet);
                }

                var ret = new TestModelDto
                {
                    Id = test.Id,
                    Description = test.Description,
                    NumberItem = test.NumberItem,
                    QuantDiasRestantes = test.quantDiasRestantes,
                    FrequencyApplication = EnumHelper.GetDescriptionFromEnumValue((EnumFrenquencyApplication)test.FrequencyApplication),
                    ApplicationEndDate = test.ApplicationEndDate.ToString("dd/MM/yyyy"),
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
        public async Task<JsonResult> LoadTestItensByTestId(long test_id, int page, int pageItens)
        {
            try
            {
                var itens = await LoadItensAsync(test_id, page, pageItens);
                await LoadAlternativesByItensAsync(test_id, itens);
                return Json(new { success = true, itens }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar os dados da prova." }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public async Task<JsonResult> LoadStudentCorrectionAsync(long test_id, long alu_id, long tur_id)
        {
            try
            {
                var studentCorrection = await _studentCorrectionBusiness.Get(alu_id, test_id, tur_id, SessionFacade.UsuarioLogado.Usuario.ent_id);
                if (studentCorrection is null)
                    return Json(new { success = true, answers = new StudentCorrectionModelDto() }, JsonRequestBehavior.AllowGet);

                var answers = studentCorrection.Answers
                    .Select(x => new AnswerModelDto
                    {
                        AlternativeId = x.AnswerChoice,
                        Changed = false,
                        ItemId = x.Item_Id
                    })
                    .ToList();

                var result = new StudentCorrectionModelDto
                {
                    LastAnswer = studentCorrection.OrdemUltimaResposta,
                    Done = studentCorrection.provaFinalizada ?? false,
                    Answers = answers
                };

                return Json(new { success = true, studentCorrection = result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar os dados da prova." }, JsonRequestBehavior.AllowGet);
            }
        }

        private async Task<IEnumerable<ItemModelDto>> LoadItensAsync(long test_id, int page, int pageItens)
        {
            var blockItems = await testBusiness.GetItemsByTestAsync(test_id, SessionFacade.UsuarioLogado.Usuario.usu_id, page, pageItens);
            if (!blockItems?.Any() ?? true) return null;

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
                if (requestRevoke != null)
                {
                    item.Revoked = requestRevoke.Situation;
                    item.ItemSituation = requestRevoke.Situation;
                    item.RequestRevoke_Id = requestRevoke.Id;
                    item.Justification = requestRevoke.Justification;
                }

                if (bi.Item.BaseText != null)
                {
                    item.BaseTextDescription = bi.Item.BaseText.Description;
                    item.BaseTextId = bi.Item.BaseText.Id;
                }

                return item;
            }).ToList();
        }

        private async Task LoadAlternativesByItensAsync(long test_id, IEnumerable<ItemModelDto> itens)
        {
            if (!itens?.Any() ?? true) return;
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
                    Correct = x.Correct,
                    Selected = false
                })
                .ToList();

            foreach (var item in itens)
            {
                var artenativesOfItem = alternativesModel.Where(x => x.Item_Id == item.Id).ToList();
                item.Alternatives.AddRange(artenativesOfItem);
            }
        }
    }
}