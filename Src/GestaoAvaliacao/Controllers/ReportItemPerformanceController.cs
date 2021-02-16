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
    public class ReportItemPerformanceController : Controller
    {
        private Pager _pager;
        private readonly IReportItemPerformanceBusiness _reportItemPerformanceBusiness;
        private readonly ISkillBusiness _skillBusiness;
        private readonly IFileBusiness _fileBusiness;
        private readonly IBaseTextBusiness _baseTextBusiness;

        public ReportItemPerformanceController(IReportItemPerformanceBusiness reportItemPerformanceBusiness, ISkillBusiness skillBusiness, 
            IFileBusiness fileBusiness, IBaseTextBusiness baseTextBusiness)
        {
            _reportItemPerformanceBusiness = reportItemPerformanceBusiness;
            _skillBusiness = skillBusiness;
            _fileBusiness = fileBusiness;
            _baseTextBusiness = baseTextBusiness;
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
        public JsonResult GetDres(long test_id, long subGroup_id, long tcp_id, Guid? dre_id, int? esc_id, Guid? uad_id)
        {
            var performanceTree = _reportItemPerformanceBusiness.GetPerformanceTree(test_id, subGroup_id, tcp_id, SessionFacade.UsuarioLogado.Usuario, SessionFacade.UsuarioLogado.Grupo, dre_id, esc_id, uad_id, showBaseText: false);
            JsonResult jsonResult = Json(new { data = performanceTree, success = true }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }


        [HttpGet]
        public JsonResult GetBaseTextByItemId(long itemId)
        {
            var textoBase = _baseTextBusiness.GetBaxeTestByItemId(itemId);
            JsonResult jsonResult = Json(new { data = textoBase, success = true }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        private List<TestAverageItens> FilterDreDesempenhoItem(List<TestAverageItens> dres, Guid? uadId)
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
            var testAverageItensViewModel = _reportItemPerformanceBusiness.ObterEscolasDesempenhoItem(test_id, discipline_id, uad_id.Value, SessionFacade.UsuarioLogado.Usuario, SessionFacade.UsuarioLogado.Grupo);
            List<TestAverageItens> lst = testAverageItensViewModel.lista.ToList();

            _pager.SetTotalItens(lst.Count);
            _pager.SetTotalPages(_pager.RecordsCount / _pager.PageSize);

            var lstPaginate = (lst
                          .Skip(_pager.CurrentPage * _pager.PageSize)
                          .Take(_pager.PageSize)
                          .ToList());

            testAverageItensViewModel.lista = lstPaginate;

            return Json(testAverageItensViewModel, JsonRequestBehavior.AllowGet);
        }
        
        [HttpGet]
        public JsonResult ExportDRE(long test_id, long subGroup_id, long tcp_id, long? discipline_id, Guid? dre_id, int? esc_id, Guid? uad_id)
        {
            EntityFile ret = new EntityFile();

            try
            {
                string separator = ";";
                Parameter param = ApplicationFacade.Parameters.FirstOrDefault(p => p.Key.Equals(EnumParameterKey.CHAR_SEP_CSV.GetDescription()));
                if (param != null)
                    separator = param.Value;

                PerformanceItemViewModel performanceTree = _reportItemPerformanceBusiness.GetPerformanceTree(test_id, subGroup_id, tcp_id, SessionFacade.UsuarioLogado.Usuario, SessionFacade.UsuarioLogado.Grupo, dre_id, esc_id, uad_id, true);

                if (performanceTree != null && performanceTree.dres.Count() > 0)
                {
                    ret = _reportItemPerformanceBusiness.ExportReportDre(performanceTree.dres.ToList(), performanceTree.disciplinas.ToList(), TypeReportsPerformanceExport.Dre, separator,
                        ApplicationFacade.VirtualDirectory, ApplicationFacade.PhysicalDirectory, SessionFacade.UsuarioLogado.Usuario, discipline_id);
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
        public JsonResult ExportSchool(long test_id, Guid? uad_id, long? discipline_id)
        {
            EntityFile ret = new EntityFile();

            try
            {
                string separator = ";";
                Parameter param = ApplicationFacade.Parameters.FirstOrDefault(p => p.Key.Equals(EnumParameterKey.CHAR_SEP_CSV.GetDescription()));
                if (param != null)
                    separator = param.Value;

                TestAverageItensViewModel result = null;

                result = _reportItemPerformanceBusiness.ObterEscolasDesempenhoItem(test_id, discipline_id, uad_id.Value, SessionFacade.UsuarioLogado.Usuario, SessionFacade.UsuarioLogado.Grupo);

                if (result != null && result.lista.Count() > 0)
                {
                    ret = _reportItemPerformanceBusiness.ExportReport(result.lista.ToList(), null, TypeReportsPerformanceExport.School, separator,
                        ApplicationFacade.VirtualDirectory, ApplicationFacade.PhysicalDirectory, SessionFacade.UsuarioLogado.Usuario, discipline_id);
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
                EntityFile file = _fileBusiness.Get(Id);
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
        public JsonResult GetSkillsByDiscipline(long Id)
        {
            try
            {
                IEnumerable<Skill> lista = _skillBusiness.GetByDiscipline(Id);

                List<ModelSkillLevel> modelSkillLevels = new List<ModelSkillLevel>();

                foreach (var skill in lista)
                {
                    if (!modelSkillLevels.Any(m => m.Id == skill.ModelSkillLevel.Id))
                    {
                        ModelSkillLevel modelSkillLevel = new ModelSkillLevel();
                        modelSkillLevel.Id = skill.ModelSkillLevel.Id;
                        modelSkillLevel.Description = skill.ModelSkillLevel.Description;
                        modelSkillLevel.Level = skill.ModelSkillLevel.Level;
                        modelSkillLevels.Add(modelSkillLevel);
                    }
                }

                var listModel = modelSkillLevels.OrderBy(m => m.Level);

                if (listModel != null && listModel.Count() > 0)
                {
                    var skillList = listModel.Select(m => new
                    {
                        ModelSkillLevels = new
                        {
                            Id = m.Id,
                            Description = m.Description,
                            Skills = lista.Where(s => s.ModelSkillLevel.Id == m.Id).OrderBy(s => s.Id).Select(s => new
                            {
                                Id = s.Id,
                                Description = s.Description,
                                ParentId = s.Parent != null ? s.Parent.Id : 0
                            }).ToList()
                        }
                    }).ToList();

                    return Json(new { success = true, lista = skillList }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { success = false, type = ValidateType.alert.ToString(), message = "Habilidades não encontradas." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar o registro pesquisado." }, JsonRequestBehavior.AllowGet);
            }
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
