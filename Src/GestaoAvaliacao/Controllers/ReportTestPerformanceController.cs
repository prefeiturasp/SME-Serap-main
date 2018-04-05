using GestaoAvaliacao.App_Start;
using GestaoAvaliacao.Entities;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.MongoEntities.DTO;
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
    public class ReportTestPerformanceController : Controller
    {
        private readonly IReportTestPerformanceBusiness _reportTestPerformanceBusiness;
        private readonly IFileBusiness fileBusiness;
        private Pager _pager;

        public ReportTestPerformanceController(IReportTestPerformanceBusiness reportTestPerformanceBusiness, IFileBusiness fileBusiness)
        {
            _reportTestPerformanceBusiness = reportTestPerformanceBusiness;
            this.fileBusiness = fileBusiness;
        }

        public ActionResult IndexDRE()
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

        public ActionResult IndexSchool()
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

        [HttpGet]
        [Paginate]
        public JsonResult GetDres(long test_id, Guid? uad_id, long? discipline_id)
        {
            _pager = this.GetPager();
            var testAverageViewModel = _reportTestPerformanceBusiness.ObterDresDesempenho(test_id, discipline_id, SessionFacade.UsuarioLogado.Usuario, SessionFacade.UsuarioLogado.Grupo);
            List<TestAveragePerformanceDTO> lst = testAverageViewModel.lista.ToList();

            _pager.SetTotalItens(lst.Count());
            _pager.SetTotalPages(_pager.RecordsCount / _pager.PageSize);

            var lstPaginate = (lst
                        .Skip(_pager.CurrentPage * _pager.PageSize)
                        .Take(_pager.PageSize)
                        .ToList());

            if (uad_id != null && uad_id.HasValue && uad_id.Value != Guid.Empty)
            {
                lstPaginate = FilterDreDesempenho(lstPaginate, uad_id);
            }
            testAverageViewModel.lista = lstPaginate;

            return Json(testAverageViewModel, JsonRequestBehavior.AllowGet);
        }

        private List<TestAveragePerformanceDTO> FilterDreDesempenho(List<TestAveragePerformanceDTO> dres, Guid? uadId)
        {
            if (uadId.HasValue)
            {
                var result = dres.Where(a => a.DreId == uadId).ToList();
                if (_pager != null)
                {
                    _pager.SetTotalItens(result.Count());
                }
                return result;
            }
            return dres;
        }

        [HttpGet]
        [Paginate]
        public JsonResult GetSchools(long test_id, Guid? uad_id, long? discipline_id)
        {
            _pager = this.GetPager();
            var testAverageViewModel = _reportTestPerformanceBusiness.ObterEscolasDesempenho(test_id, discipline_id, uad_id.Value, SessionFacade.UsuarioLogado.Usuario, SessionFacade.UsuarioLogado.Grupo);
            List<TestAveragePerformanceDTO> lst = testAverageViewModel.lista.ToList();

            _pager.SetTotalItens(lst.Count);
            _pager.SetTotalPages(_pager.RecordsCount / _pager.PageSize);

            var lstPaginate = (lst
                          .Skip(_pager.CurrentPage * _pager.PageSize)
                          .Take(_pager.PageSize)
                          .ToList());

            testAverageViewModel.lista = lstPaginate;

            return Json(testAverageViewModel, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult ExportDRE(long test_id, Guid? uad_id, long? discipline_id)
        {
            EntityFile ret = new EntityFile();

            try
            {
                string separator = ";";
                Parameter param = ApplicationFacade.Parameters.FirstOrDefault(p => p.Key.Equals(EnumParameterKey.CHAR_SEP_CSV.GetDescription()));
                if (param != null)
                    separator = param.Value;

                TestAverageViewModel result = null;

                result = _reportTestPerformanceBusiness.ObterDresDesempenho(test_id, discipline_id, SessionFacade.UsuarioLogado.Usuario, SessionFacade.UsuarioLogado.Grupo);

                if (uad_id != null && uad_id.HasValue && uad_id.Value != Guid.Empty)
                {
                    result.lista = FilterDreDesempenho(result.lista.ToList(), uad_id);
                }

                if (result != null && result.lista.Count() > 0)
                {
                    ret = _reportTestPerformanceBusiness.ExportReport(result.lista.ToList(), TypeReportsPerformanceExport.Dre, separator,
                        ApplicationFacade.VirtualDirectory, ApplicationFacade.PhysicalDirectory, SessionFacade.UsuarioLogado.Usuario);
                }
                else
                {
                    ret.Validate.IsValid = false;
                    ret.Validate.Type = ValidateType.alert.ToString();
                    ret.Validate.Message = "Ainda não existem dados para exportar.";
                }
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

        [HttpGet]
        public JsonResult ExportSchool(long test_id, Guid uad_id, long? discipline_id)
        {
            EntityFile ret = new EntityFile();

            try
            {
                string separator = ";";
                Parameter param = ApplicationFacade.Parameters.FirstOrDefault(p => p.Key.Equals(EnumParameterKey.CHAR_SEP_CSV.GetDescription()));
                if (param != null)
                    separator = param.Value;

                TestAverageViewModel result = null;

                result = _reportTestPerformanceBusiness.ObterEscolasDesempenho(test_id, discipline_id, uad_id, SessionFacade.UsuarioLogado.Usuario, SessionFacade.UsuarioLogado.Grupo);

                if (result != null && result.lista.Count() > 0)
                {
                    ret = _reportTestPerformanceBusiness.ExportReport(result.lista.ToList(), TypeReportsPerformanceExport.School, separator,
                        ApplicationFacade.VirtualDirectory, ApplicationFacade.PhysicalDirectory, SessionFacade.UsuarioLogado.Usuario);
                }
                else
                {
                    ret.Validate.IsValid = false;
                    ret.Validate.Type = ValidateType.alert.ToString();
                    ret.Validate.Message = "Ainda não existem dados para exportar.";
                }
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

        // Bounded Context - Core
        private bool IsUserAdmin()
        {
            return (EnumSYS_Visao)Enum.Parse(typeof(EnumSYS_Visao),
                SessionFacade.UsuarioLogado.Grupo.vis_id.ToString()) == EnumSYS_Visao.Administracao;
        }

        private bool IsUserGestor()
        {
            return (EnumSYS_Visao)Enum.Parse(typeof(EnumSYS_Visao),
                SessionFacade.UsuarioLogado.Grupo.vis_id.ToString()) == EnumSYS_Visao.Gestao;
        }

        private bool IsUserAdmUnity()
        {
            return (EnumSYS_Visao)Enum.Parse(typeof(EnumSYS_Visao),
                SessionFacade.UsuarioLogado.Grupo.vis_id.ToString()) == EnumSYS_Visao.UnidadeAdministrativa;
        }

        private bool IsUserTeacher()
        {
            return (EnumSYS_Visao)Enum.Parse(typeof(EnumSYS_Visao),
                SessionFacade.UsuarioLogado.Grupo.vis_id.ToString()) == EnumSYS_Visao.Individual;
        }

    }
}