using GestaoAvaliacao.App_Start;
using GestaoAvaliacao.Entities;
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
	public class ReportItemController : Controller
	{
		private readonly IItemBusiness itemBusiness;
		private readonly ISkillBusiness skillBusiness;
		private readonly IACA_TipoCurriculoPeriodoBusiness tipoCurriculoPeriodoBusiness;

        public ReportItemController(IItemBusiness itemBusiness, ISkillBusiness skillBusiness, IACA_TipoCurriculoPeriodoBusiness tipoCurriculoPeriodoBusiness)
		{
			this.itemBusiness = itemBusiness;
			this.skillBusiness = skillBusiness;
			this.tipoCurriculoPeriodoBusiness = tipoCurriculoPeriodoBusiness;
		}

		[ActionAuthorizeAttribute(Permission.CreateOrUpdate)]
		public ActionResult Index()
		{
			return View();
		}

		#region Read

		[HttpGet]
		public JsonResult load_reportItemType(int id, int situacao, long typeLevelEducation)
		{
			try
			{
				if (id == -1)
					id = 0;

				List<ItemReportItemType> lista = itemBusiness._GetItemType(id, situacao, SessionFacade.UsuarioLogado.Usuario.ent_id, typeLevelEducation);

				if (lista != null && lista.Count() > 0)
				{
					var reportItemType = lista.Select(x => new
					{
						Description = x.Description,
						Value = x.Total
					});

					return Json(new { success = true, lista = reportItemType }, JsonRequestBehavior.AllowGet);
				}
				else
					return Json(new { success = true, lista, type = ValidateType.alert.ToString(), message = "Gráficos não encontrados." }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				LogFacade.SaveError(ex);
				return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar gerar os gráficos." }, JsonRequestBehavior.AllowGet);
			}
		}

		[HttpGet]
		public JsonResult load_reportItemLevel(int id, int situacao, long typeLevelEducation)
		{
			try
			{
				if (id == -1)
					id = 0;

				List<ItemReportItemLevel> lista = itemBusiness._GetItemLevel(id, situacao, SessionFacade.UsuarioLogado.Usuario.ent_id, typeLevelEducation);

				if (lista != null && lista.Count() > 0)
				{
					var reportItemLevel = lista.Select(x => new
					{
						Description = x.Description,
						Value = x.Total
					});

					return Json(new { success = true, lista = reportItemLevel }, JsonRequestBehavior.AllowGet);
				}
				else
					return Json(new { success = true, lista, type = ValidateType.alert.ToString(), message = "Gráficos não encontrados." }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				LogFacade.SaveError(ex);
				return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar gerar os gráficos." }, JsonRequestBehavior.AllowGet);
			}
		}

		[HttpGet]
		public JsonResult load_reportItem(long typeLevelEducation)
		{
			try
			{
				List<ItemReportItem> lista = itemBusiness._GetItem(SessionFacade.UsuarioLogado.Usuario.ent_id, typeLevelEducation);

				if (lista != null && lista.Count() > 0)
				{
					var reportItem = lista.Select(x => new
					{
						Description = x.Description,
						Value = x.Total,
						Total = x.TotalItem
					});

					return Json(new { success = true, lista = reportItem }, JsonRequestBehavior.AllowGet);
				}
				else
					return Json(new { success = true, lista, type = ValidateType.alert.ToString(), message = "Gráficos não encontrados." }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				LogFacade.SaveError(ex);
				return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar gerar os gráficos." }, JsonRequestBehavior.AllowGet);
			}
		}

		[HttpGet]
		public JsonResult load_reportItemCurriculumGrade(int id, int situacao, long typeLevelEducation)
		{
			try
			{
				if (id == -1)
					id = 0;

				List<ItemReportItemCurriculumGrade> lista = itemBusiness._GetItemCurriculumGrade(id, situacao, SessionFacade.UsuarioLogado.Usuario.ent_id, typeLevelEducation);
				IEnumerable<ACA_TipoCurriculoPeriodo> listCurriculumGrades = tipoCurriculoPeriodoBusiness.GetAllTypeCurriculumGrades();

				if (lista != null && lista.Count() > 0)
				{
					var reportItemCurriculumGrade = lista.Select(x => new
					{
						Description = listCurriculumGrades.FirstOrDefault(c => c.tcp_id == x.Id).tcp_descricao,
						Value = x.Total
					}).ToList();

					return Json(new { success = true, lista = reportItemCurriculumGrade }, JsonRequestBehavior.AllowGet);
				}
				else
					return Json(new { success = true, lista, type = ValidateType.alert.ToString(), message = "Gráficos não encontrados." }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				LogFacade.SaveError(ex);
				return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar gerar os gráficos." }, JsonRequestBehavior.AllowGet);
			}
		}

		[HttpGet]
		public JsonResult load_reportItemSituation(int id, string inicio, string fim, long typeLevelEducation)
		{
			try
			{
				if (id == -1)
					id = 0;

				List<ItemReportItemSituation> lista = itemBusiness._GetItemSituation(id, inicio, fim, SessionFacade.UsuarioLogado.Usuario.ent_id, typeLevelEducation);

				if (lista != null && lista.Count() > 0)
				{
					var reportItemSituation = lista.Select(x => new
					{
						Description = x.Description,
						Value = x.Total
					});

					return Json(new { success = true, lista = reportItemSituation }, JsonRequestBehavior.AllowGet);
				}
				else
					return Json(new { success = true, lista, type = ValidateType.alert.ToString(), message = "Gráficos não encontrados." }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				LogFacade.SaveError(ex);
				return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar gerar os gráficos." }, JsonRequestBehavior.AllowGet);
			}
		}

		[HttpGet]
		public JsonResult load_reportItemSkill(int id, int skill, long typeLevelEducation)
		{
			try
			{
				if (id == -1)
					id = 0;

				IEnumerable<ItemReportItemSkill> lista = skillBusiness.GetBySkillReport(id, skill, SessionFacade.UsuarioLogado.Usuario.ent_id, typeLevelEducation);

				if (lista != null && lista.Count() > 0)
				{
					var reportItemSkill = lista.Select(x => new
					{
						ModelDescription = x.ModelDescription,
						Description = x.Description,
						Code = x.Code,
						Value = x.Total
					});

					return Json(new { success = true, lista = reportItemSkill }, JsonRequestBehavior.AllowGet);
				}
				else
					return Json(new { success = true, lista, type = ValidateType.alert.ToString(), message = "Gráficos não encontrados." }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				LogFacade.SaveError(ex);
				return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar gerar os gráficos." }, JsonRequestBehavior.AllowGet);
			}
		}

		[HttpGet]
		public JsonResult load_reportItemSkillOneLevel(int id, int matrizId, long typeLevelEducation)
		{
			try
			{
				if (id == -1)
					id = 0;

				IEnumerable<ItemReportItemSkill> lista = skillBusiness.GetBySkillReportOneLevel(id, matrizId, SessionFacade.UsuarioLogado.Usuario.ent_id, typeLevelEducation);

				if (lista != null && lista.Count() > 0)
				{
					var reportItemSkill = lista.Select(x => new
					{
						ModelDescription = x.ModelDescription,
						Description = x.Description,
						Code = x.Code,
						Value = x.Total
					});

					return Json(new { success = true, lista = reportItemSkill }, JsonRequestBehavior.AllowGet);
				}
				else
					return Json(new { success = true, lista, type = ValidateType.alert.ToString(), message = "Gráficos não encontrados." }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				LogFacade.SaveError(ex);
				return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar gerar os gráficos." }, JsonRequestBehavior.AllowGet);
			}
		}

		[HttpGet]
		public JsonResult GetByMatrix(long Id)
		{
			try
			{
				IEnumerable<Skill> lista = skillBusiness.GetByMatrix(Id);

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
					return Json(new { success = false, type = ValidateType.alert.ToString(), message = "Matriz não possui nível para exibir." }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				LogFacade.SaveError(ex);
				return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar o registro pesquisado." }, JsonRequestBehavior.AllowGet);
			}
		}

		#endregion
	}
}