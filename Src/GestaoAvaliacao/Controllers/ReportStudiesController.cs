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
    public JsonResult Save(HttpPostedFileBase file, string Name, int TypeGroup, string Addressee, string Link)
    {
        try
        {
            var entity = new ReportStudies
            {
                Name = file?.FileName,
                TypeGroup = TypeGroup,
                Addressee = Addressee,
                Link = Link
            };

            if (entity == null)
                throw new Exception("Entidade não pode ser nula");
            var ret = reportStudiesBusiness.Save(entity);


            return Json(new { success = ret, message = ret ? null : "Erro ao salvar arquivo." }, JsonRequestBehavior.AllowGet);
        }
        catch (Exception ex)
        {
            LogFacade.SaveError(ex);
            return Json(new { success = false, message = "Erro ao salvar arquivo." }, JsonRequestBehavior.AllowGet);
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
                    Grupo = entity.TypeGroup,
                    Destinatario = entity.Addressee,
                    DataUpload = entity.CreateDate.ToShortDateString(),
                    Link = entity.Link
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
            return Json(new { success = false , message = "Erro ao deletar  arquivo." }, JsonRequestBehavior.AllowGet);
        }
   
    }
}