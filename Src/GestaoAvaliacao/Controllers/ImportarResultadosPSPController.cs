using GestaoAvaliacao.App_Start;
using GestaoAvaliacao.Entities;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.Util;
using GestaoAvaliacao.Util.Videos;
using GestaoAvaliacao.WebProject.Facade;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace GestaoAvaliacao.Controllers
{
    [Authorize]
    [AuthorizeModule]
    public class ImportarResultadosPSPController : Controller
    {

        private readonly IResultadoPspBusiness resultadoPspBusiness;

        public ImportarResultadosPSPController(IResultadoPspBusiness resultadoPspBusiness)
        {
            this.resultadoPspBusiness = resultadoPspBusiness;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Paginate]
        public JsonResult ObterImportacoes(string codigoOuNomeArquivo)
        {
            try
            {

                Pager pager = this.GetPager();
                IEnumerable<ArquivoResultadoPsp> result = resultadoPspBusiness.ObterImportacoes(ref pager, codigoOuNomeArquivo);

                if (result != null)
                {
                    var ret = result.Select(entity => new
                    {
                        Id = entity.Id,
                        NomeArquivo = entity.NomeArquivo,
                        Status = entity.Status,
                        CreateDate = entity.CreateDate.ToShortDateString()
                    });

                    return Json(new { success = true, lista = ret, pageSize = pager.PageSize }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { success = false, lista = "" }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar as importações pesquisados." }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}