using GestaoAvaliacao.App_Start;
using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Entities.Enumerator;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.Util;
using GestaoAvaliacao.WebProject.Facade;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace GestaoAvaliacao.Controllers
{
    [Authorize]
	[AuthorizeModule]
    public class ModelEvaluationMatrixController : Controller
    {
        private readonly IModelEvaluationMatrixBusiness modelEvaluationMatrixBusiness;

        public ModelEvaluationMatrixController(IModelEvaluationMatrixBusiness modelEvaluationMatrixBusiness)
        {
            this.modelEvaluationMatrixBusiness = modelEvaluationMatrixBusiness;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult IndexForm()
        {
            return View("IndexForm");
        }

        public ActionResult IndexList()
        {
            return View("Index");
        }

        #region Read

        [HttpGet]
        public JsonResult Find(int Id)
        {
            try
            {
                ModelEvaluationMatrix modelEvaluationMatrix = modelEvaluationMatrixBusiness.Get(Id);
                return Json(new { success = true, typeLevelEducation = modelEvaluationMatrix }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar o modelo de matriz de avaliação pesquisado." }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetModelEvaluation(long Id)
        {
            try
            {
                ModelEvaluationMatrix entity = modelEvaluationMatrixBusiness.GetModelEvaluationMatrix(Id);

                if (entity != null)
                {
                    var ret = new
                    {
                        Id = entity.Id,
                        Description = entity.Description,
                        State = entity.State,
                        ModelSkillLevels = entity.ModelSkillLevels.Where(i => i.State == (Byte)EnumState.ativo).Select(a => new
                        {
                            Id = a.Id,
                            Description = a.Description,
                            Level = a.Level,
                            LastLevel = a.LastLevel
                        }).OrderBy(i => i.Level).ToList()
                    };

                    return Json(new { success = true, modelList = ret }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { success = false, type = ValidateType.alert.ToString(), message = "Modelo de matriz de avaliação não encontrado." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar o modelo da matriz de avaliação pesquisado." }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult Load()
        {
            try
            {
                IEnumerable<ModelEvaluationMatrix> lista = modelEvaluationMatrixBusiness.Load(SessionFacade.UsuarioLogado.Usuario.ent_id);

                if (lista != null && lista.Count() > 0)
                {
                    var modelEvaluationMatrixList = lista.Select(x => new
                    {
                        Id = x.Id,
                        Description = x.Description,
                    });

                    return Json(new { success = true, lista = modelEvaluationMatrixList }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { success = false, type = ValidateType.alert.ToString(), message = "Modelo da matriz de avaliação não encontrado." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar o modelo da matriz de avaliação pesquisado." }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        [Paginate]
        public JsonResult LoadPaginate()
        {
            try
            {
                Pager pager = this.GetPager();

                IEnumerable<ModelEvaluationMatrix> lista = modelEvaluationMatrixBusiness.LoadPaginate(ref pager, SessionFacade.UsuarioLogado.Usuario.ent_id);

                if (lista != null && lista.Count() > 0)
                {
                    var modelEvaluationMatrixList = lista.Select(x => new
                    {
                        Id = x.Id,
                        Description = x.Description,
                        modelState = x.State,
                        LevelCount = x.ModelSkillLevels.Where(c => c.State == (Byte)EnumState.ativo).Count()
                    }).ToList();

                    return Json(new { success = true, lista = modelEvaluationMatrixList }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { success = true, lista }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar o(s) modelo(os) de matriz de avaliação pesquisado." }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        [Paginate]
        public JsonResult Search(String search = null, int levelQntd = 0)
        {
            try
            {
                Pager pager1 = this.GetPager();
                IEnumerable<ModelEvaluationMatrix> lista = ((search != null && !string.IsNullOrEmpty(search)) || levelQntd != 0) ? modelEvaluationMatrixBusiness.Search(ref pager1, SessionFacade.UsuarioLogado.Usuario.ent_id, search, levelQntd) : modelEvaluationMatrixBusiness.LoadPaginate(ref pager1, SessionFacade.UsuarioLogado.Usuario.ent_id);

                if (lista != null && lista.Count() > 0)
                {
                    var modelEvaluationMatrixList = lista.Select(x => new
                    {
                        Id = x.Id,
                        Description = x.Description,
                        modelState = x.State,
                        levelCount = x.ModelSkillLevels.Where(c => c.State == (Byte)EnumState.ativo).Count()
                    }).ToList();

                    return Json(new { success = true, lista = modelEvaluationMatrixList }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { success = true, lista }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar o(s) modelo(os) de matriz de avaliação pesquisado." }, JsonRequestBehavior.AllowGet);
            }
        }
        
        #endregion 

        #region Write

        [HttpPost]
        public JsonResult Save(ModelEvaluationMatrix entity)
        {
            try
            {
                if (entity.Id > 0)
                {
                    entity = modelEvaluationMatrixBusiness.Update(entity, SessionFacade.UsuarioLogado.Usuario.ent_id);
                }
                else
                {
                    entity = modelEvaluationMatrixBusiness.Save(entity, SessionFacade.UsuarioLogado.Usuario.ent_id);
                }
            }
            catch (Exception ex)
            {
                entity.Validate.IsValid = false;
                entity.Validate.Type = ValidateType.error.ToString();
                entity.Validate.Message = string.Format("Erro ao {0} o modelo de matriz de avaliação.", entity.Id > 0 ? "alterar" : "salvar");

                LogFacade.SaveError(ex);
            }

            return Json(new { success = entity.Validate.IsValid, type = entity.Validate.Type, message = entity.Validate.Message }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Delete(long Id)
        {
            ModelEvaluationMatrix entity = new ModelEvaluationMatrix();

            try
            {
                entity = modelEvaluationMatrixBusiness.Delete(Id, SessionFacade.UsuarioLogado.Usuario.ent_id);
            }
            catch (Exception ex)
            {
                entity.Validate.IsValid = false;
                entity.Validate.Type = ValidateType.error.ToString();
                entity.Validate.Message = "Erro ao tentar excluir o modelo de matriz de avaliação.";
                LogFacade.SaveError(ex);
            }

            return Json(new { success = entity.Validate.IsValid, type = entity.Validate.Type, message = entity.Validate.Message }, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}