using GestaoAvaliacao.App_Start;
using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Entities.Enumerator;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.Util;
using GestaoAvaliacao.WebProject.Facade;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Web.Mvc;

namespace GestaoAvaliacao.Controllers
{
    [Authorize]
    [AuthorizeModule]
    public class SubjectController : Controller
    {
        private readonly ISubjectBusiness subjectBusiness;
        private readonly IKnowledgeAreaBusiness knowledgeAreaBusiness;
        private readonly IDisciplineBusiness disciplineBusiness;

        public SubjectController(ISubjectBusiness subjectBusiness, IKnowledgeAreaBusiness knowledgeAreaBusiness, IDisciplineBusiness disciplineBusiness)
        {
            this.subjectBusiness = subjectBusiness;
            this.knowledgeAreaBusiness = knowledgeAreaBusiness;
            this.disciplineBusiness = disciplineBusiness;
        }

        public ActionResult Form()
        {
            return View();
        }

        public ActionResult List()
        {
            return View();
        }

        #region Read
        
        [HttpGet]
        public JsonResult GetSubject(long Id)
        {
            try
            {
                Subject entity = subjectBusiness.GetSubject(Id);

                if (entity != null)
                {
                    var ret = new
                    {
                        Id = entity.Id,
                        Description = entity.Description,
                        State = entity.State,
                        SubSubject = entity.SubSubjects.Where(i => i.State == (Byte)EnumState.ativo).Select(a => new
                        {
                            Id = a.Id,
                            Description = a.Description
                        }),
                        KnowledgeArea = entity.KnowledgeAreas.Where(i => i.State == (Byte)EnumState.ativo).Select(a => new
                        {
                            Id = a.Id,
                            Description = a.Description
                        }),
                        Discipline = entity.Disciplines.Where(i => i.State == (Byte)EnumState.ativo).Select(a => new
                        {
                            Id = a.Id,
                            Description = a.Description
                        })
                    };

                    return Json(new { success = true, modelList = ret }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { success = false, type = ValidateType.alert.ToString(), message = "Assunto e subassunto não encontrado." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar o assunto e subassunto pesquisado." }, JsonRequestBehavior.AllowGet);
            }
        }


        [Route("loadallknowledgeareaactive")]
        [HttpGet]
        public JsonResult LoadAllKnowledgeAreaActive(string description)
        {
            try
            {
                return Json(knowledgeAreaBusiness.LoadAllKnowledgeAreaActive(description, SessionFacade.UsuarioLogado.Usuario.ent_id), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return null;
            }
        }

        [Route("loaddisciplinebyknowledgearea")]
        [HttpGet]
        public JsonResult LoadDisciplineByKnowledgeArea(string description, string knowledgeAreas)
        {
            try
            {
                return Json(disciplineBusiness.LoadDisciplineByKnowledgeArea(description, knowledgeAreas, SessionFacade.UsuarioLogado.Usuario.ent_id), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return null;
            }
        }

        [HttpGet]
        [Paginate]
        public JsonResult SearchSubjects(string assunto, string subassunto)
        {
            try
            {
                var visao = (EnumSYS_Visao)Enum.Parse(typeof(EnumSYS_Visao), SessionFacade.UsuarioLogado.Grupo.vis_id.ToString());

                Pager pager = this.GetPager();
                var lista = subjectBusiness.SearchSubjects(assunto, subassunto, SessionFacade.UsuarioLogado.Usuario.ent_id, ref pager);

                if (lista != null)
                {
                    var ret = lista.Select(entity => new
                    {
                        Discipline = entity
                    }).ToList();

                    return Json(new { success = true, lista = ret }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { success = false, type = ValidateType.alert.ToString(), message = "Assuntos e subassuntos não encontrados." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar itens pesquisados." }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult LoadSubjectBySubsubject(long idSubsubject)
        {
            try
            {
                var subject = subjectBusiness.LoadSubjectBySubsubject(idSubsubject);

                if (subject != null)
                {
                    var assunto = new
                    {
                        Id = subject.Id,
                        Description = subject.Description,
                        Subsubject_Id = subject.Subsubject_Id,
                        Subsubject_Description = subject.Subsubject_Description
                    };

                    return Json(new { success = true, assunto = assunto }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { success = false, type = ValidateType.alert.ToString(), message = "Assunto não encontrado." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar o assunto." }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region Write

        [HttpPost]
        public JsonResult Save(Subject entity)
        {
            try
            {
                if (entity.Id > 0)
                {
                    entity = subjectBusiness.Update(entity, SessionFacade.UsuarioLogado.Usuario.ent_id);
                }
                else
                {
                    entity = subjectBusiness.Save(entity, SessionFacade.UsuarioLogado.Usuario.ent_id);
                }
            }
            catch (Exception ex)
            {
                entity.Validate.IsValid = false;
                entity.Validate.Type = ValidateType.error.ToString();
                entity.Validate.Message = string.Format("Erro ao {0} o assunto e subassunto.", entity.Id > 0 ? "alterar" : "salvar");

                LogFacade.SaveError(ex);
            }

            return Json(new { success = entity.Validate.IsValid, type = entity.Validate.Type, message = entity.Validate.Message }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Delete(long Id)
        {
            Subject entity = new Subject();

            try
            {
                entity = subjectBusiness.Delete(Id, SessionFacade.UsuarioLogado.Usuario.ent_id);
            }
            catch (Exception ex)
            {
                entity.Validate.IsValid = false;
                entity.Validate.Type = ValidateType.error.ToString();
                entity.Validate.Message = "Erro ao tentar excluir o assunto.";
                LogFacade.SaveError(ex);
            }

            return Json(new { success = entity.Validate.IsValid, type = entity.Validate.Type, message = entity.Validate.Message }, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}