using GestaoAvaliacao.App_Start;
using GestaoAvaliacao.Entities;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.Util;
using GestaoAvaliacao.WebProject.Facade;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace GestaoAvaliacao.Controllers
{
    [Authorize]
    [AuthorizeModule]
    public class ImportarResultadosPSPController : Controller
    {

        private readonly IResultadoPspBusiness resultadoPspBusiness;
        private readonly IFileBusiness fileBusiness;

        public ImportarResultadosPSPController(IResultadoPspBusiness resultadoPspBusiness, IFileBusiness fileBusiness)
        {
            this.resultadoPspBusiness = resultadoPspBusiness;
            this.fileBusiness = fileBusiness;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Paginate]
        public JsonResult ObterImportacoes(string codigoOuNomeArquivoOuTipo)
        {
            try
            {
                Pager pager = this.GetPager();
                IEnumerable<ArquivoResultadoPsp> result = resultadoPspBusiness.ObterImportacoes(ref pager, codigoOuNomeArquivoOuTipo);

                if (result != null)
                {
                    var ret = result.Select(entity => new
                    {
                        entity.Id,
                        NomeArquivo = entity.NomeOriginalArquivo,
                        Status = ((EnumStatusImportResulProvaSp)entity.State).GetDescription(),
                        CreateDate = entity.CreateDate.ToShortDateString(),
                        entity.Tipo
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

        [HttpGet]
        public JsonResult ObterTiposResultadoPspAtivos()
        {
            try
            {

                Pager pager = this.GetPager();
                IEnumerable<TipoResultadoPsp> result = resultadoPspBusiness.ObterTiposResultadoPspAtivos();

                if (result != null)
                {
                    var ret = result.Select(entity => new
                    {
                        Codigo = entity.Codigo,
                        Nome = entity.Nome
                    });

                    return Json(new { success = true, lista = ret }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { success = false, lista = "" }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar os tipos de resultado PSP." }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult ImportarArquivoResultado(HttpPostedFileBase file, int codigoTipoResultado)
        {
            try
            {
                var arquivoResultado = new ArquivoResultadoPsp();
                var tipoResultado = resultadoPspBusiness.ObterTipoResultadoPorCodigo(codigoTipoResultado);

                var guidArquivo = Guid.NewGuid();
                arquivoResultado.CodigoTipoResultado = codigoTipoResultado;
                arquivoResultado.NomeArquivo = $"{guidArquivo}.csv";
                arquivoResultado.NomeOriginalArquivo = file.FileName;

                var retorno = resultadoPspBusiness.ImportarArquivoResultado(arquivoResultado, file);
                
                return Json(new { success = retorno, message = retorno ? null : "Erro ao importar resultados." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Json(new { success = false, message = "Erro ao importar resultados." }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public void BaixarModelo(int codigoTipoResultado)
        {
            try
            {
                var arquivoResultado = new ArquivoResultadoPsp();
                var tipoResultado = resultadoPspBusiness.ObterTipoResultadoPorCodigo(codigoTipoResultado);
                Response.Clear();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", $"attachment; filename=Modelo{tipoResultado.NomeTabelaProvaSp.Trim()}.csv");
                Response.Charset = "";
                Response.ContentType = "application/text";
                Response.Output.Write(tipoResultado.ModeloArquivo);
                Response.Flush();
                Response.End();
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                throw ex;
            }
        }
    }
}