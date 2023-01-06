using GestaoAvaliacao.App_Start;
using GestaoAvaliacao.Entities;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.Util;
using GestaoAvaliacao.WebProject.Facade;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EntityFile = GestaoAvaliacao.Entities.File;

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
                        NomeArquivo = entity.NomeOriginalArquivo,
                        Status = entity.State,
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
            EntityFile entity = new EntityFile();
            var arquivoResultado = new ArquivoResultadoPsp();

            try
            {

                var tipoResultado = resultadoPspBusiness.ObterTipoResultadoPorCodigo(codigoTipoResultado);

                UploadModel upload = new UploadModel
                {
                    ContentLength = file.ContentLength,
                    ContentType = file.ContentType,
                    InputStream = null,
                    Stream = file.InputStream,
                    FileName = file.FileName,
                    VirtualDirectory = ApplicationFacade.VirtualDirectory,
                    PhysicalDirectory = $"{ApplicationFacade.PhysicalDirectory}\\ResultadosPSP\\{tipoResultado.NomeTabelaProvaSp}",
                    FileType = EnumFileType.File,
                    UsuId = SessionFacade.UsuarioLogado.Usuario.usu_id
                };

                arquivoResultado.CodigoTipoResultado = codigoTipoResultado;
                arquivoResultado.NomeOriginalArquivo = file.FileName;
                entity = fileBusiness.Upload(upload);

                if (entity != null && entity.Validate.IsValid)
                {
                    var resultado = resultadoPspBusiness.ImportarArquivoResultado(arquivoResultado, entity);
                    if (!resultado)
                    {
                        fileBusiness.Delete(entity.Id);
                        return Json(new { success = false }, JsonRequestBehavior.AllowGet);
                    }

                    return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                entity.Validate.IsValid = false;
                entity.Validate.Type = ValidateType.error.ToString();
                entity.Validate.Message = "Erro ao realizar o upload do arquivo.";
                LogFacade.SaveError(ex);
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}