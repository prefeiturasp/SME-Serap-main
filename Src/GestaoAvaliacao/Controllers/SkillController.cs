using GestaoAvaliacao.App_Start;
using GestaoAvaliacao.Entities;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.MongoEntities.DTO;
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
	public class SkillController : Controller
	{
		private readonly ISkillBusiness skillBusiness;
		private readonly IModelSkillLevelBusiness modelSkillLevelBusiness;
        private readonly IEvaluationMatrixBusiness evaluationMatrixBusiness;

        public SkillController(ISkillBusiness skillBusiness, IModelSkillLevelBusiness modelSkillLevelBusiness, IEvaluationMatrixBusiness evaluationMatrixBusiness)
		{
			this.skillBusiness = skillBusiness;
			this.modelSkillLevelBusiness = modelSkillLevelBusiness;
            this.evaluationMatrixBusiness = evaluationMatrixBusiness;
        }

		public ActionResult Index()
		{
			return View();
		}

		#region Read

		[HttpGet]
		public JsonResult Find(int Id)
		{
			Skill skill = skillBusiness.Get(Id);
			return Json(new { success = true, skill = skill }, JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		public JsonResult FindParent(int Id)
		{
			object skill = skillBusiness.GetParent(Id);

			return Json(new { success = true, skill = skill }, JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		[Paginate]
		public JsonResult Search(String search)
		{
			try
			{
				Pager pager1 = this.GetPager();
				IEnumerable<Skill> lista = search != null ? skillBusiness.Search(search, ref pager1) : skillBusiness.Load(ref pager1);

				if (lista != null && lista.Count() > 0)
				{
					var skillList = lista.Select(x => new
					{
						Id = x.Id,
						Description = x.Description,
					});

					return Json(new { success = true, lista = skillList }, JsonRequestBehavior.AllowGet);
				}
				else
					return Json(new { success = false, type = ValidateType.alert.ToString(), message = "A pesquisa não retornou resultados." }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				LogFacade.SaveError(ex);
				return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao pesquisar o nível." }, JsonRequestBehavior.AllowGet);
			}
		}

		[HttpGet]
		[Paginate]
		public JsonResult SearchByMatrix(long evaluationMatrixId, long modelSkillLevelId, long parentId, string acao)
		{
			try
			{
				IEnumerable<Skill> lista;
				Pager pager1 = this.GetPager();

				if (!acao.Equals("back"))
					lista = skillBusiness.SearchByMatrix(evaluationMatrixId, modelSkillLevelId, parentId, ref pager1);
				else
				{
					Skill skill = new Skill();
					skill = skillBusiness.Get(parentId);

					if (skill.Parent == null)
						skill.Parent = new Skill();

					lista = skillBusiness.SearchByMatrix(evaluationMatrixId, modelSkillLevelId, skill.Parent.Id, ref pager1);
				}

				if (lista != null && lista.Count() > 0)
				{
					var skillList = lista.Select(x => new
					{
						Id = x.Id,
						Description = x.Code + " - " + x.Description,
						LastLevel = x.LastLevel,
						Parent = x.Parent == null ? 0 : x.Parent.Id,
						ModelSkillLevel = new
						{
							Id = x.ModelSkillLevel.Id,
							Description = x.ModelSkillLevel.Description,
							Level = x.ModelSkillLevel.Level
						},
						CognitiveCompetence = x.CognitiveCompetence == null ? null :
						new
						{
							Id = x.CognitiveCompetence.Id,
							Description = x.CognitiveCompetence.Description
						}

					}).OrderBy(b => b.Description);

					return Json(new { success = true, lista = skillList }, JsonRequestBehavior.AllowGet);
				}
				else
					return Json(new { success = false, type = ValidateType.alert.ToString(), message = "A pesquisa não retornou resultados." }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				LogFacade.SaveError(ex);
				return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao pesquisar níveis da matriz." }, JsonRequestBehavior.AllowGet);
			}
		}

		[HttpGet]
		[Paginate]
		public JsonResult LoadByMatrix(long evaluationMatrixId, long modelSkillLevelId, long parentId)
		{
			try
			{
				Pager pager = this.GetPager();
				IEnumerable<Skill> lista = skillBusiness.LoadByMatrix(evaluationMatrixId, modelSkillLevelId, parentId, ref pager);

				if (lista != null && lista.Count() > 0)
				{
					var skillList = lista.Select(x => new
					{
						Id = x.Id,
						Description = x.Code + " - " + x.Description,
						LastLevel = x.LastLevel,
						Parent = x.Parent == null ? 0 : x.Parent.Id,
					});

					return Json(new { success = true, lista = skillList }, JsonRequestBehavior.AllowGet);
				}
				else
					return Json(new { success = false, type = ValidateType.alert.ToString(), message = "Níveis não encontrados." }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				LogFacade.SaveError(ex);
				return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao pesquisar os níveis da matriz." }, JsonRequestBehavior.AllowGet);
			}
		}

		[HttpGet]
		[Paginate]
		public JsonResult Load()
		{
			try
			{
				Pager pager = this.GetPager();

				IEnumerable<Skill> lista = skillBusiness.Load(ref pager);

				var skillList = lista.Select(x => new
				{
					Id = x.Id,
					Description = x.Description,
				});

				if (lista != null && lista.Count() > 0)
					return Json(new { success = true, lista = skillList }, JsonRequestBehavior.AllowGet);
				else
				{
					return Json(new { success = false, type = ValidateType.alert.ToString(), message = "Níveis não encontrados." }, JsonRequestBehavior.AllowGet);
				}
			}
			catch (Exception ex)
			{
				LogFacade.SaveError(ex);
				return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao carregar os níveis." }, JsonRequestBehavior.AllowGet);

			}
		}

		public JsonResult GetByMatrix(long Id)
		{
			try
			{
				IEnumerable<Skill> lista = skillBusiness.GetByMatrix(Id);

				if (lista != null && lista.Count() > 0)
				{
					IEnumerable<ModelSkillLevel> modelSkillLevel = modelSkillLevelBusiness.GetById(lista.FirstOrDefault().ModelSkillLevel.Id);

					IEnumerable<ModelSkillLevel> modelSkillLevels = modelSkillLevelBusiness.GetByMatrixModel(modelSkillLevel.FirstOrDefault().ModelEvaluationMatrix.Id);

					var listModel = modelSkillLevels.OrderBy(m => m.Level);

					if (listModel != null && listModel.Count() > 0)
					{
						var skillList = listModel.Select(m => new
						{
							ModelSkillLevels = new
							{
								Id = m.Id,
								Description = m.Description,
								Skills = lista.Where(s => s.ModelSkillLevel.Id == m.Id && s.Parent == null).OrderBy(s => s.Id).Select(s => new
								{
									Id = s.Id,
									Description = s.Code + " - " + s.Description,
									ParentId = s.Parent != null ? s.Parent.Id : 0,
									LastLevel = s.LastLevel
								}).OrderBy(a => a.Description).ToList()
							}
						}).ToList();

						return Json(new { success = true, lista = skillList }, JsonRequestBehavior.AllowGet);
					}
				}

				return Json(new { success = false, type = ValidateType.alert.ToString(), message = "A matriz não possui níveis cadastrados." }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				LogFacade.SaveError(ex);
				return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao pesquisar níveis da matriz." }, JsonRequestBehavior.AllowGet);
			}
		}

        public JsonResult GetByParent(long Id)
		{
			try
			{
				IEnumerable<Skill> lista = skillBusiness.GetByParent(Id);

				if (lista != null && lista.Count() > 0)
				{
					var skillList = lista.Select(s => new
					{
						Skills = new
						{
							Id = s.Id,
							Description = s.Code + " - " + s.Description,
							LastLevel = s.LastLevel
						}
					}).OrderBy(a => a.Skills.Description).ToList();

					return Json(new { success = true, lista = skillList }, JsonRequestBehavior.AllowGet);
				}
				else
					return Json(new { success = false, type = ValidateType.alert.ToString(), message = "Nível não encontrado." }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				LogFacade.SaveError(ex);
				return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao pesquisar o nível." }, JsonRequestBehavior.AllowGet);
			}
		}

        [HttpGet]
        public JsonResult GetComboByDiscipline(long Id)
        {
            try
            {
                IEnumerable<Skill> lista = skillBusiness.GetComboByDiscipline(Id);

                if (lista != null && lista.Count() > 0)
                {
                    var evaluationMatrixList = lista.Select(m => new
                    {
                        m.Id,
                        m.Description,
                        m.Code
                    }).ToList();

                    return Json(new { success = true, lista = evaluationMatrixList }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { success = false, type = ValidateType.alert.ToString(), message = "Habilidade não encontrada." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao pesquisar a habilidade." }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region Write

        [HttpPost]
		public JsonResult Save(Skill entity)
		{
			try
			{
				if (entity.Id > 0)
				{
					entity = skillBusiness.Update(entity.Id, entity);
				}
				else
				{
					entity = skillBusiness.Save(entity);
				}
			}
			catch (Exception ex)
			{
				entity.Validate.IsValid = false;
				entity.Validate.Type = ValidateType.error.ToString();
				entity.Validate.Message = string.Format("Erro ao {0} o nível.", entity.Id > 0 ? "alterar" : "salvar");

				LogFacade.SaveError(ex);
			}

			return Json(new { success = entity.Validate.IsValid, type = entity.Validate.Type, message = entity.Validate.Message }, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		public JsonResult SaveRange(List<Skill> listSkill)
		{
			Skill entity = new Skill();

			try
			{
				entity = skillBusiness.SaveRange(listSkill);
			}
			catch (Exception ex)
			{
				entity.Validate.IsValid = false;
				entity.Validate.Type = ValidateType.error.ToString();
				entity.Validate.Message = "Erro ao salvar o nível.";

				LogFacade.SaveError(ex);
			}

			return Json(new { success = entity.Validate.IsValid, type = entity.Validate.Type, message = entity.Validate.Message }, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		public JsonResult Delete(int Id)
		{
			Skill entity = new Skill();

			try
			{
				entity = skillBusiness.Delete(Id);
			}
			catch (Exception ex)
			{
				entity.Validate.IsValid = false;
				entity.Validate.Type = ValidateType.error.ToString();
				entity.Validate.Message = "Erro ao tentar excluir o nível.";
				LogFacade.SaveError(ex);
			}

			return Json(new { success = entity.Validate.IsValid, type = entity.Validate.Type, message = entity.Validate.Message }, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		public JsonResult DeleteByMatrix(int Id, int evaluationMatrixId)
		{
			Skill entity = new Skill();

			try
			{
				entity = skillBusiness.DeleteByMatrix(Id, evaluationMatrixId);
			}
			catch (Exception ex)
			{
				entity.Validate.IsValid = false;
				entity.Validate.Type = ValidateType.error.ToString();
				entity.Validate.Message = "Erro ao tentar excluir o nível.";
				LogFacade.SaveError(ex);
			}

			return Json(new { success = entity.Validate.IsValid, type = entity.Validate.Type, message = entity.Validate.Message }, JsonRequestBehavior.AllowGet);
		}

		#endregion
	}
}