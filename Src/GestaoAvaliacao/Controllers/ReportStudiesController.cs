using GestaoAvaliacao.App_Start;
using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Entities.Enumerator;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.Util;
using GestaoAvaliacao.WebProject.Facade;
using ProvaSP.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GestaoEscolar.IBusiness;
using ProvaSP.Model.Entidades;

[Authorize]
[AuthorizeModule]
public class ReportStudiesController : Controller
{
    private readonly IReportStudiesBusiness reportStudiesBusiness;
    private readonly ISYS_UnidadeAdministrativaBusiness unidadeAdministrativaBusiness;
    private readonly IESC_EscolaBusiness escolaBusiness;

    public ReportStudiesController(IReportStudiesBusiness reportStudiesBusiness,
        ISYS_UnidadeAdministrativaBusiness unidadeAdministrativaBusiness, IESC_EscolaBusiness escolaBusiness)
    {
        this.reportStudiesBusiness = reportStudiesBusiness;
        this.unidadeAdministrativaBusiness = unidadeAdministrativaBusiness;
        this.escolaBusiness = escolaBusiness;
    }

    public ActionResult Index()
    {
        return View();
    }

    private bool CheckReportEstudiesPermissions(EnumTypeGroup typeGroup, string uadCodigoDestinatario)
    {
        var usuarioLogado = SessionFacade.UsuarioLogado;

        Usuario usuario = null;
        var dres= Enumerable.Empty<string>();
        var ues = Enumerable.Empty<string>();
        var dresDasUes = Enumerable.Empty<string>();
        var permissaoDre = false;
        var permissaoUe = false;

        if (usuarioLogado != null)
        {
            usuario = DataUsuario.RetornarUsuario(usuarioLogado.Usuario.usu_login, "");
            dres = unidadeAdministrativaBusiness.LoadDRESimple(usuarioLogado.Usuario, usuarioLogado.Grupo).Select(c => c.uad_codigo);
            ues = escolaBusiness.ListarEscolasPorCodigosDres(dres).Select(c => c.EscCodigo).Distinct();
            dresDasUes = escolaBusiness.ListarEscolasPorCodigosDres(dres).Select(c => c.DreCodigo).Distinct();

            permissaoDre = usuario.AcessoNivelDRE && (dres.Contains(uadCodigoDestinatario ?? string.Empty) ||
                                                      dresDasUes.Contains(uadCodigoDestinatario ?? string.Empty));

            permissaoUe = usuario.AcessoNivelEscola && ues.Contains(uadCodigoDestinatario ?? string.Empty);

        }

        switch (typeGroup)
        {
            case EnumTypeGroup.SME:
                return usuario != null && usuario.AcessoNivelSME;
            case EnumTypeGroup.DRE:
                return usuario != null && (usuario.AcessoNivelSME || permissaoDre);
            case EnumTypeGroup.UE:
                return usuario != null && (usuario.AcessoNivelSME || permissaoDre || permissaoUe);
            case EnumTypeGroup.GERAL:
                return usuarioLogado != null;
            case EnumTypeGroup.PUBLICO:
            default:
                return typeGroup == EnumTypeGroup.PUBLICO;
        }
    }

    [AllowAnonymous]
    [HttpGet]
    public JsonResult CheckReportStudiesExists(long id)
    {
        var valid = new Validate
        {
            IsValid = false,
            Type = ValidateType.alert.ToString()
        };

        try
        {
            var entity = reportStudiesBusiness.GetById(id);
            if (entity == null)
                valid.Message = "Relatório de estudos não existe";
            else
            {
                var typeGroup = entity.TypeGroup ?? (int)EnumTypeGroup.PUBLICO;
                if (!CheckReportEstudiesPermissions((EnumTypeGroup)typeGroup, entity.UadCodigoDestinatario))
                    valid.Message = "Você não possui permissão de acesso ao relatório de estudos.";
                else
                {
                    var link = entity.Link;
                    if (string.IsNullOrEmpty(link))
                        valid.Message = "Link do relatório de estudos não existe";
                    else
                    {
                        var filePath = new Uri(link).AbsolutePath.Replace("Files/", string.Empty);
                        var physicalPath = string.Concat(ApplicationFacade.PhysicalDirectorySme,
                            filePath.Replace("/", "\\"));
                        var decodedUrl = HttpUtility.UrlDecode(physicalPath);

                        if (!System.IO.File.Exists(decodedUrl))
                            valid.Message = "Caminho do relatório de estudos não existe";
                        else
                        {
                            valid.IsValid = true;
                            valid.Type = string.Empty;
                        }
                    }
                }
            }
        }
        catch (Exception e)
        {
            LogFacade.SaveError(e);
            valid.IsValid = false;
            valid.Type = ValidateType.error.ToString();
            valid.Message = "Erro ao tentar encontrar o relatório de estudos.";
        }

        return Json(new { success = valid.IsValid, type = valid.Type, message = valid.Message },
            JsonRequestBehavior.AllowGet);
    }

    [AllowAnonymous]
    [HttpGet]
    public void GetReportStudies(long id)
    {
        var entity = reportStudiesBusiness.GetById(id);
        if (entity == null)
            return;

        var typeGroup = entity.TypeGroup ?? (int)EnumTypeGroup.PUBLICO;
        if (!CheckReportEstudiesPermissions((EnumTypeGroup)typeGroup, entity.UadCodigoDestinatario))
        {
            System.Web.HttpContext.Current.Response.Redirect("/", false);
            System.Web.HttpContext.Current.ApplicationInstance.CompleteRequest();
        }
        else
        {
            var link = entity.Link;
            if (string.IsNullOrEmpty(link))
                return;

            var fileExtension = Path.GetExtension(link);
            var fileExtensionEnum = EnumExtensions.GetValueFromDescription<EnumFileExtension>(fileExtension);

            string contentType;
            switch (fileExtensionEnum)
            {
                case EnumFileExtension.Html:
                    contentType = EnumFileContentType.Html.GetDescription();
                    break;
                case EnumFileExtension.Csv:
                    contentType = EnumFileContentType.Csv.GetDescription();
                    break;
                default:
                    throw new NotImplementedException("Extensão de arquivo não implementada.");
            }

            var filePath = new Uri(link).AbsolutePath.Replace("Files/", string.Empty);
            var physicalPath = string.Concat(ApplicationFacade.PhysicalDirectorySme, filePath.Replace("/", "\\"));

            var decodedUrl = HttpUtility.UrlDecode(physicalPath);
            if (!System.IO.File.Exists(decodedUrl))
                return;

            var file = System.IO.File.ReadAllBytes(decodedUrl);

            Response.Clear();
            Response.ContentType = contentType;
            Response.Buffer = true;
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.BinaryWrite(file);
            Response.End();
            Response.Close();
        }
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
                    entity.UadCodigoDestinatario,
                    DataUpload = entity.CreateDate.ToString(),
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
    public JsonResult ListarGrupos()
    {
        try
        {
            return Json(new { success = true, lista = reportStudiesBusiness.ListarGrupos() }, JsonRequestBehavior.AllowGet);
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