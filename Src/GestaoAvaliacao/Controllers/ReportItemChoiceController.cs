using GestaoAvaliacao.App_Start;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.Util;
using GestaoAvaliacao.WebProject.Facade;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using EntityFile = GestaoAvaliacao.Entities.File;

namespace GestaoAvaliacao.Controllers
{
    [Authorize]
    [AuthorizeModule]
    public class ReportItemChoiceController : Controller
    {
        private readonly IReportItemChoiceBusiness _reportItemPerformanceBusiness;
        private readonly IFileBusiness fileBusiness;

        public ReportItemChoiceController(IReportItemChoiceBusiness reportItemPerformanceBusiness, IFileBusiness fileBusiness)
        {
            _reportItemPerformanceBusiness = reportItemPerformanceBusiness;
            this.fileBusiness = fileBusiness;
        }

        public ActionResult Index()
        {
            if (SessionFacade.UsuarioLogado != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Busca as médias de escolha por alternativa de cada item em uma prova
        /// </summary>
        /// <param name="test_Id">Id da prova</param>
        /// <param name="dre_id">Id da DRE</param>
        /// <param name="esc_id">Id da escola</param>
        /// <returns>Dto com o percentual de escolha de cada alternativa em cada item da prova</returns>
        [HttpGet]
        public async Task<JsonResult> GetItemPercentageChoicePerAlternative(long test_id, Guid? dre_id, int? esc_id, long? discipline_id)
        {
            try
            {
                var itemsPercentageChoicePerAlternative = await _reportItemPerformanceBusiness
                                                                    .GetItemPercentageChoiceByAlternative(test_id, discipline_id, dre_id, esc_id);

                if (itemsPercentageChoicePerAlternative != null && itemsPercentageChoicePerAlternative.Items.Any())
                {
                    return Json(new { success = true, data = itemsPercentageChoicePerAlternative }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { success = false, type = ValidateType.alert.ToString(), message = "Não foram encontrados dados para os filtros selecionados" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar buscar as médias de escolhas das alternativas por item." }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Exporta o relatório para CSV
        /// </summary>
        /// <param name="test_id">Id da prova</param>
        /// <param name="dre_id">Id da DRE</param>
        /// <param name="esc_id">Id da escola</param>
        /// <returns>Dados para download do CSV</returns>
        [HttpGet]
        public async Task<JsonResult> ExportReport(int test_id, long? discipline_id, Guid? dre_id, int? esc_id)
        {
            EntityFile ret = new EntityFile();

            try
            {
                string separator = ApplicationFacade
                                    .Parameters
                                    .FirstOrDefault(p => p.Key.Equals(EnumParameterKey.CHAR_SEP_CSV.GetDescription()))
                                    .Value ?? ";";

                ret = await _reportItemPerformanceBusiness.ExportReport(test_id, discipline_id, dre_id, esc_id, separator,
                                                                  ApplicationFacade.VirtualDirectory, ApplicationFacade.PhysicalDirectory);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                ret.Validate.Type = ValidateType.error.ToString();
                ret.Validate.IsValid = false;
                ret.Validate.Message = "Erro ao obter os dados.";
            }

            return Json(new { success = ret.Validate.IsValid, type = ret.Validate.Type, message = ret.Validate.Message, file = ret }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public void DownloadFile(long Id)
        {
            bool redirect = false;
            try
            {
                EntityFile file = fileBusiness.Get(Id);
                if (file != null)
                {
                    string filePath = new Uri(file.Path).AbsolutePath.Replace("Files/", string.Empty);
                    string physicalPath = string.Concat(ApplicationFacade.PhysicalDirectory, filePath.Replace("/", "\\"));
                    string decodedUrl = HttpUtility.UrlDecode(physicalPath);

                    if (System.IO.File.Exists(decodedUrl))
                    {
                        System.IO.FileStream fs = System.IO.File.Open(decodedUrl, System.IO.FileMode.Open);
                        byte[] btFile = new byte[fs.Length];
                        fs.Read(btFile, 0, Convert.ToInt32(fs.Length));
                        fs.Close();

                        Response.Clear();
                        Response.AddHeader("Content-disposition", "attachment; filename=" + file.OriginalName);
                        Response.ContentType = file.ContentType;
                        Response.BinaryWrite(btFile);
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
    }
}