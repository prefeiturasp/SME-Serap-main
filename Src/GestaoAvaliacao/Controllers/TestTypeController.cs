using GestaoAvaliacao.App_Start;
using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Entities.Enumerator;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.Util;
using GestaoAvaliacao.WebProject.Facade;
using GestaoEscolar.Entities;
using GestaoEscolar.IBusiness;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace GestaoAvaliacao.Controllers
{
    [Authorize]
    [AuthorizeModule]
    public class TestTypeController : Controller
    {
        private readonly ITestTypeBusiness testTypeBusiness;
        private readonly ITestTypeCourseCurriculumGradeBusiness testTypeCourseCurriculumGradeBusiness;
        private readonly IACA_TipoCurriculoPeriodoBusiness tipoCurriculoPeriodoBusiness;
        private readonly IACA_TipoNivelEnsinoBusiness levelEducationBusiness;
        private readonly IItemLevelBusiness itemLevelBusiness;

        public TestTypeController(ITestTypeBusiness testTypeBusiness, ITestTypeCourseCurriculumGradeBusiness testTypeCourseCurriculumGradeBusiness,
            IACA_TipoCurriculoPeriodoBusiness tipoCurriculoPeriodoBusiness, IACA_TipoNivelEnsinoBusiness levelEducationBusiness,
            IItemLevelBusiness itemLevelBusiness)
        {
            this.testTypeBusiness = testTypeBusiness;
            this.testTypeCourseCurriculumGradeBusiness = testTypeCourseCurriculumGradeBusiness;
            this.tipoCurriculoPeriodoBusiness = tipoCurriculoPeriodoBusiness;
            this.levelEducationBusiness = levelEducationBusiness;
            this.itemLevelBusiness = itemLevelBusiness;
        }

        public ActionResult List()
        {
            return View();
        }

        public ActionResult Form()
        {
            return View();
        }

        #region Read

        [HttpGet]
        public JsonResult GetFrequencyApplicationList()
        {
            try
            {
                var ret = Enum.GetValues(typeof(EnumFrenquencyApplication)).Cast<EnumFrenquencyApplication>().Select(v => new
                {
                    Id = (int)v,
                    Description = EnumHelper.GetDescriptionFromEnumValue(v),
                    Parent = EnumHelper.GetParentIdFromEnumValue(v)
                }).ToList();

                if (ret != null)
                {
                    return Json(new { success = true, lista = ret }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { success = false, type = ValidateType.alert.ToString(), message = "Frequências de aplicação não encontradas." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar as frequências de aplicação." }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetFrequencyApplicationParentList()
        {
            try
            {
                var ret = Enum.GetValues(typeof(EnumFrenquencyApplication)).Cast<EnumFrenquencyApplication>().Select(v => new
                {
                    Id = (int)v,
                    Description = EnumHelper.GetDescriptionFromEnumValue(v),
                    Parent = EnumHelper.GetParentIdFromEnumValue(v)
                }).Where(v => v.Parent == 0).ToList();

                if (ret != null)
                {
                    return Json(new { success = true, lista = ret }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { success = false, type = ValidateType.alert.ToString(), message = "Frequências de aplicação não encontradas." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar as frequências de aplicação." }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetFrequencyApplicationChildList(int parentId)
        {
            try
            {
                var ret = Enum.GetValues(typeof(EnumFrenquencyApplication)).Cast<EnumFrenquencyApplication>().Select(v => new
                {
                    Id = (int)v,
                    Description = EnumHelper.GetDescriptionFromEnumValue(v),
                    Parent = EnumHelper.GetParentIdFromEnumValue(v)
                }).Where(v => v.Parent == parentId).ToList();

                if (ret != null)
                {
                    return Json(new { success = true, lista = ret }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { success = false, type = ValidateType.alert.ToString(), message = "Frequências de aplicação não encontradas." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar as frequências de aplicação." }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult Find(int Id)
        {
            try
            {
                TestType testType = testTypeBusiness.Get(Id, SessionFacade.UsuarioLogado.Usuario.ent_id);

                var retorno = new
                {
                    Id = testType.Id,
                    Description = testType.Description,
                    //CourseId = testType.CourseId,
                    ModelTest_Id = testType.ModelTest_Id,
                    FormatType = new
                    {
                        Id = testType.FormatType == null ? 0 : testType.FormatType.Id,
                        Description = testType.FormatType == null ? string.Empty : testType.FormatType.Description
                    },
                    ItemType = new
                    {
                        Id = testType.ItemType == null ? 0 : testType.ItemType.Id,
                        Description = testType.ItemType == null ? string.Empty : testType.ItemType.Description,
                        IsDefault = (testType.ItemType != null) && testType.ItemType.IsDefault
                    },
                    TestTypeItemLevel = (from il in testType.TestTypeItemLevel
                                         where il.State == (Byte)EnumState.ativo
                                         select new
                                         {
                                             Id = il.Id,
                                             Value = il.Value,
                                             IdItem = il.ItemLevel.Id,
                                             Description = il.ItemLevel.Description
                                         }).ToList(),
                    FrequencyApplication = testType.FrequencyApplication,
                    Bib = testType.Bib,
                    Global = testType.Global,
                    TypeLevelEducation = levelEducationBusiness.GetCustom(testType.TypeLevelEducationId),
                    Disabled = testType.TestTypeCourses.FirstOrDefault(p => p.State == (Byte)EnumState.ativo) != null
                };

                return Json(new { success = true, testType = retorno }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao carregar tipo de prova." }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        [Paginate]
        public JsonResult Load()
        {
            try
            {
                Pager pager = this.GetPager();
                IEnumerable<TestType> lista = testTypeBusiness.Load(ref pager, SessionFacade.UsuarioLogado.Usuario.ent_id);

                if (lista != null && lista.Count() > 0)
                {
                    var testTypeList = lista.Select(x => new
                    {
                        Id = x.Id,
                        Description = x.Description,
                    });

                    return Json(new { success = true, lista = testTypeList }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { success = false, type = ValidateType.alert.ToString(), message = "Tipo de prova não encontrado." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar tipo de prova pesquisado." }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Retorna os tipos de prova que o usuário tem acesso de acordo com seu grupo de permissões no sistema.
        /// Se usuário é admin retorna todos tipos de prova, senão retorna os que não são Global.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult LoadByUserGroup()
        {
            try
            {
                IEnumerable<TestType> lista = testTypeBusiness.LoadFiltered(SessionFacade.UsuarioLogado.Usuario.ent_id, SessionFacade.UsuarioLogado.Grupo.vis_id == (int)EnumSYS_Visao.Administracao);

                if (lista != null && lista.Count() > 0)
                {
                    var list = new
                    {
                        testTypeList = lista.Select(x => new
                        {
                            Id = x.Id,
                            Description = x.Description,
                            FrequencyApplication = x.FrequencyApplication
                        }),
                        Bib = lista.Any(p => p.Bib)
                    };

                    return Json(new { success = true, lista = list }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { success = false, type = ValidateType.alert.ToString(), message = "Tipo de prova não encontrado." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar tipo de prova pesquisado." }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Retorna os tipos de prova que o usuário tem acesso de acordo com seu grupo de permissões no sistema.
        /// Se usuário é admin retorna todos tipos de prova, senão retorna os que não são Global.
        /// Método utilizado na consulta de provas
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult LoadByUserGroupSearchTest()
        {
            try
            {
                IEnumerable<TestType> lista = testTypeBusiness.LoadByUserGroup(SessionFacade.UsuarioLogado.Usuario.ent_id, SessionFacade.UsuarioLogado.Grupo.vis_id == (int)EnumSYS_Visao.Administracao);

                if (lista != null && lista.Count() > 0)
                {
                    var list = new
                    {
                        testTypeList = lista.Select(x => new
                        {
                            Id = x.Id,
                            Description = x.Description,
                            TypeLevelEducationId = x.TypeLevelEducationId,
                            FrequencyApplication = x.FrequencyApplication
                        }),
                        Bib = lista.Any(p => p.Bib)
                    };

                    return Json(new { success = true, lista = list }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { success = false, type = ValidateType.alert.ToString(), message = "Tipo de prova não encontrado." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar tipo de prova pesquisado." }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Método usado no cadastro de prova.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult FindTest(int Id)
        {
            try
            {
                TestType testType = testTypeBusiness.Get(Id, SessionFacade.UsuarioLogado.Usuario.ent_id);
                List<TestTypeCourseCurriculumGrade> testTypeCourseCurriculumGrade = testTypeCourseCurriculumGradeBusiness.GetCurriculumGradesByTestType((int)testType.Id);
                IEnumerable<ACA_TipoCurriculoPeriodo> listCurriculumGrades = tipoCurriculoPeriodoBusiness.GetAllTypeCurriculumGrades();
                IEnumerable<ItemLevel> listItemLevel = itemLevelBusiness.LoadLevels(SessionFacade.UsuarioLogado.Usuario.ent_id);

                var retorno = new
                {
                    Id = testType.Id,
                    TestTypeItemLevel = listItemLevel.Select(x => new
                    {
                        Id = testType.TestTypeItemLevel.FirstOrDefault(p => p.ItemLevel.Id == x.Id) != null ? testType.TestTypeItemLevel.FirstOrDefault(p => p.ItemLevel.Id == x.Id).Id : 0,
                        Value = testType.TestTypeItemLevel.FirstOrDefault(p => p.ItemLevel.Id == x.Id && p.State == (byte)EnumState.ativo) != null ? testType.TestTypeItemLevel.FirstOrDefault(p => p.ItemLevel.Id == x.Id && p.State == (byte)EnumState.ativo).Value : null,
                        IdItem = x.Id,
                        Description = x.Description
                    }).ToList(),
                    FrequencyApplication = testType.FrequencyApplication,
                    Bib = testType.Bib,
                    FormatType = testType.FormatType != null ? new { Id = testType.FormatType.Id, Description = testType.FormatType.Description } : null,
                    ItemType = testType.ItemType != null ? new
                    {
                        Id = testType.ItemType.Id,
                        Description = testType.ItemType.Description,
                        IsDefault = testType.ItemType.IsDefault,
                        QuantityAlternative = testType.ItemType.QuantityAlternative != null ? testType.ItemType.QuantityAlternative : 0
                    } : null,
                    TypeCurriculumGrade = testTypeCourseCurriculumGrade.Select(p => new
                    {
                        Id = listCurriculumGrades.FirstOrDefault(a => a.tcp_id == p.TypeCurriculumGradeId)?.tcp_id,
                        Description = listCurriculumGrades.FirstOrDefault(a => a.tcp_id == p.TypeCurriculumGradeId)?.tcp_descricao,
                        Order = listCurriculumGrades.FirstOrDefault(a => a.tcp_id == p.TypeCurriculumGradeId)?.tcp_ordem
                    }),
                    TypeLevelEducation = levelEducationBusiness.GetCustom(testType.TypeLevelEducationId)
                };

                return Json(new { success = true, testType = retorno }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar tipo de prova pesquisado." }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        [Paginate]
        public JsonResult Search(String search)
        {
            try
            {
                Pager pager = this.GetPager();
                IEnumerable<TestType> lista = search != null ? testTypeBusiness.Search(search, ref pager, SessionFacade.UsuarioLogado.Usuario.ent_id) : testTypeBusiness.Load(ref pager, SessionFacade.UsuarioLogado.Usuario.ent_id);

                if (lista != null && lista.Count() > 0)
                {
                    var testTypeList = lista.Select(x => new
                    {
                        Id = x.Id,
                        Description = x.Description,
                    });

                    return Json(new { success = true, lista = testTypeList }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { success = false, type = ValidateType.alert.ToString(), message = "Tipo de prova não encontrado." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar tipo de prova pesquisado." }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult ExistsTestAssociated(long Id)
        {
            try
            {
                bool hasTest = testTypeBusiness.ExistsTestAssociated(Id);

                return Json(new { success = true, block = hasTest }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar tipo de prova pesquisado." }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region Write

        [HttpPost]
        public JsonResult Save(TestType entity)
        {
            try
            {
                if (entity.Id > 0)
                {
                    entity = testTypeBusiness.Update(entity.Id, entity, SessionFacade.UsuarioLogado.Usuario.ent_id);
                }
                else
                {
                    entity = testTypeBusiness.Save(entity, SessionFacade.UsuarioLogado.Usuario.ent_id);
                }
            }
            catch (Exception ex)
            {
                entity.Validate.IsValid = false;
                entity.Validate.Type = ValidateType.error.ToString();
                entity.Validate.Message = string.Format("Erro ao {0} o tipo de prova.", entity.Id > 0 ? "alterar" : "salvar");

                LogFacade.SaveError(ex);
            }

            return Json(new
            {
                success = entity.Validate.IsValid,
                type = entity.Validate.Type,
                message = entity.Validate.Message,
                testTypeID = entity.Id,
                testTypeItemLevel = (from il in entity.TestTypeItemLevel
                                     where il.State == (Byte)EnumState.ativo
                                     select new
                                     {
                                         Id = il.Id,
                                         Value = il.Value,
                                         IdItem = il.ItemLevel.Id,
                                         Description = il.ItemLevel.Description
                                     }).ToList()
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Delete(int Id)
        {
            TestType entity = new TestType();

            try
            {
                entity = testTypeBusiness.Delete(Id, SessionFacade.UsuarioLogado.Usuario.ent_id);
            }
            catch (Exception ex)
            {
                entity.Validate.IsValid = false;
                entity.Validate.Type = ValidateType.error.ToString();
                entity.Validate.Message = "Erro ao tentar excluir o tipo de prova.";
                LogFacade.SaveError(ex);
            }

            return Json(new { success = entity.Validate.IsValid, type = entity.Validate.Type, message = entity.Validate.Message }, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}