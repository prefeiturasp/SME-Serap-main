using GestaoAvaliacao.App_Start;
using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Entities.Enumerator;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.Util;
using GestaoAvaliacao.WebProject.Facade;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

[Authorize]
[AuthorizeModule]
public class ReportStudiesController : Controller
{
    private readonly IReportStudiesBusiness reportStudiesBusiness;

    public ReportStudiesController(IReportStudiesBusiness reportStudiesBusiness)
    {
        this.reportStudiesBusiness = reportStudiesBusiness;
    }
    public ActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public JsonResult Save(HttpPostedFileBase file, int? TypeGroup, string uadCodigoDestinatario)
    {
        try
        {
            var entity = new ReportStudies
            {
                Name = file?.FileName,
                TypeGroup = TypeGroup,
                UadCodigoDestinatario = uadCodigoDestinatario,
            };            

            UploadModel upload = new UploadModel
            {
                ContentLength = file.ContentLength,
                ContentType = file.ContentType,
                InputStream = null,
                Stream = file.InputStream,
                FileName = file.FileName,
                VirtualDirectory = ApplicationFacade.VirtualDirectory,
                PhysicalDirectory = ApplicationFacade.PhysicalDirectory,
                FileType = EnumFileType.File,
                UsuId = SessionFacade.UsuarioLogado.Usuario.usu_id
            };

            if (entity == null)
                throw new Exception("Entidade não pode ser nula");
            var ret = reportStudiesBusiness.Save(entity, upload);

            return Json(new { success = ret, message = ret ? null : "Erro ao salvar arquivo." }, JsonRequestBehavior.AllowGet);
        }
        catch (Exception ex)
        {
            LogFacade.SaveError(ex);
            return Json(new { success = false, message = "Erro ao salvar arquivo." }, JsonRequestBehavior.AllowGet);
        }
    }

    [HttpPost]
    public JsonResult Update(long id, int? tipoGrupo, string uadCodigoDestinatario)
    {
        try
        {
            var entity = new ReportStudies
            {
                Id = id,
                TypeGroup = tipoGrupo,
                UadCodigoDestinatario = uadCodigoDestinatario,
            };            

            var ret = reportStudiesBusiness.Update(entity);

            return Json(new { success = ret, message = ret ? null : "Erro ao alterar arquivo." }, JsonRequestBehavior.AllowGet);
        }
        catch (Exception ex)
        {
            LogFacade.SaveError(ex);
            return Json(new { success = false, message = "Erro ao alterar arquivo.", type = ValidateType.error.ToString() }, JsonRequestBehavior.AllowGet);
        }
    }

    [HttpGet]
    [Paginate]
    public JsonResult ListReportStudies(string searchFilter)
    {
        try
        {
            Pager pager = this.GetPager();
            IEnumerable<ReportStudies> result = reportStudiesBusiness.ListPaginated(ref pager, searchFilter);

            if (result != null)
            {
                var ret = result.Select(entity => new
                {
                    Codigo = entity.Id,
                    NomeArquivo = entity.Name,
                    TipoGrupo = entity.TypeGroup != null ? (EnumTypeGroup)entity.TypeGroup : (EnumTypeGroup?)null,
                    STipoGrupo = entity.TypeGroup != null ? entity.TypeGroup.ToString() : null,
                    Grupo = entity.TypeGroup != null ? ((EnumTypeGroup)entity.TypeGroup).GetDescription() : "",
                    Destinatario = entity.Addressee,
                    UadCodigoDestinatario = entity.UadCodigoDestinatario,
                    DataUpload = entity.CreateDate.ToString(),
                    Link = entity.Link,
                    ObjDestinatario = new { id = $"{entity.UadCodigoDestinatario?.ToString()}", text = $"{entity.Addressee?.ToString()}" }
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
            return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar os estudos de relatórios pesquisados." }, JsonRequestBehavior.AllowGet);
        }
    }

    [Route("listargrupos")]
    [HttpGet]

    public JsonResult ListarGrupos(string description)
    {
        try
        {
            return Json(new { success = true, lista = reportStudiesBusiness.ListarGrupos(description) }, JsonRequestBehavior.AllowGet);
        }
        catch (Exception ex)
        {
            LogFacade.SaveError(ex);
            return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar listar os grupos." }, JsonRequestBehavior.AllowGet);
        }
    }

    [Route("listardestinatarios")]
    [HttpGet]
    public JsonResult ListarDestinatarios(string filtroDesc, EnumTypeGroup? tipoGrupo)
    {
        try
        {
            return Json(new { succes = true, lista = reportStudiesBusiness.ListarDestinatarios(SessionFacade.UsuarioLogado.Usuario, SessionFacade.UsuarioLogado.Grupo, tipoGrupo, filtroDesc) }, JsonRequestBehavior.AllowGet);
        }
        catch (Exception ex)
        {
            LogFacade.SaveError(ex);
            return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar listar os destinatários." }, JsonRequestBehavior.AllowGet);
        }
    }

    [HttpGet]
    public JsonResult ListarDestinatariosEditarInicial(string filtroDesc, EnumTypeGroup? tipoGrupo)
    {
        try
        {
            var result = reportStudiesBusiness.ListarDestinatarios(SessionFacade.UsuarioLogado.Usuario, SessionFacade.UsuarioLogado.Grupo, tipoGrupo, filtroDesc);
            return Json(new { success = true, message = string.Empty, lista = result }, JsonRequestBehavior.AllowGet);
        }
        catch (Exception ex)
        {
            LogFacade.SaveError(ex);
            return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar listar os destinatários." }, JsonRequestBehavior.AllowGet);
        }
    }

    [HttpPost]
    public JsonResult Delete(long id)
    {
        try
        {
            var ret = reportStudiesBusiness.DeleteById(id);
            return Json(new { success = ret, message = ret ? null : "Erro ao deletar  arquivo." }, JsonRequestBehavior.AllowGet);
        }
        catch (Exception ex)
        {
            LogFacade.SaveError(ex);
            return Json(new { success = false, message = "Erro ao deletar  arquivo." }, JsonRequestBehavior.AllowGet);
        }

    }

    [HttpPost]
    public JsonResult ImportCsv(HttpPostedFileBase file)
    {
        var b = new BinaryReader(file.InputStream);
        var binData = b.ReadBytes(file.ContentLength);
        var result = System.Text.Encoding.UTF8.GetString(binData);

        var splitRowResult = result.Substring(0, StringHelper.PositionOfNewLine(result)).Trim()
            .Replace("\"", string.Empty).Split(';');

        if (string.IsNullOrEmpty(splitRowResult.ToString()))
            return Json(new { success = false, message = "Erro ao importar csv." }, JsonRequestBehavior.AllowGet);

        b.BaseStream.Position = 0;

        try
        {

            reportStudiesBusiness.ImportCsv(file, SessionFacade.UsuarioLogado.Usuario, SessionFacade.UsuarioLogado.Grupo, out var retorno);
            return Json(new { success = true, retorno, message = "Importação realizada com sucesso!." }, JsonRequestBehavior.AllowGet);
        }
        catch (Exception ex)
        {
            LogFacade.SaveError(ex);
            return Json(new { success = false, retorno = "", message = ex.Message }, JsonRequestBehavior.AllowGet);
        }
    }



}