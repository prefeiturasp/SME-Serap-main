using GestaoAvaliacao.App_Start;
using GestaoAvaliacao.Entities;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.Util;
using GestaoAvaliacao.WebProject.Facade;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace GestaoAvaliacao.Controllers
{
    public class ModelTestController : Controller
	{
		private readonly IModelTestBusiness modelTestBusiness;

		public ModelTestController(IModelTestBusiness modelTestBusiness)
		{
			this.modelTestBusiness = modelTestBusiness;
		}

		// GET: ModelTest
		public ActionResult Index()
		{
			return View();
		}
		public ActionResult IndexForm()
		{
			return View();
		}

		#region Read
		[HttpGet]
		public JsonResult Find(int Id)
		{
			try
			{
				ModelTest modelTest = modelTestBusiness.Get(Id);
				modelTest.StudentInformationHtml = string.Empty;
				modelTest.HeaderHtml = string.Empty;
				modelTest.FooterHtml = string.Empty;

				return Json(new { success = true, modelTest = modelTest }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				LogFacade.SaveError(ex);
				return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar modelo de prova pesquisada." }, JsonRequestBehavior.AllowGet);
			}
		}

		[HttpGet]
		public JsonResult FindSimple()
		{
			try
			{
				var modelTestList = modelTestBusiness.FindSimple(SessionFacade.UsuarioLogado.Usuario.ent_id)
					.Select(m =>
						new { Id = m.Id, Description = m.Description }
					);

				return Json(new { success = true, lista = modelTestList }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				LogFacade.SaveError(ex);
				return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar modelo de prova pesquisada." }, JsonRequestBehavior.AllowGet);
			}
		}

		[HttpGet]
		[Paginate]
		public JsonResult Search(String search)
		{
			try
			{
				Pager pager = this.GetPager();
				IEnumerable<ModelTest> lista = modelTestBusiness.Search(ref pager, SessionFacade.UsuarioLogado.Usuario.ent_id, search);

				if (lista != null && lista.Count() > 0)
				{
					var modelTestList = lista.Select(x => new
					{
						Id = x.Id,
						Description = x.Description,
						DefaultModel = x.DefaultModel
					});

					return Json(new { success = true, lista = modelTestList }, JsonRequestBehavior.AllowGet);
				}
				else
					return Json(new { success = true, lista }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				LogFacade.SaveError(ex);
				return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar modelo de prova pesquisada." }, JsonRequestBehavior.AllowGet);
			}
		}
		#endregion

		#region Write
		[HttpPost]
		public JsonResult Save(ModelTest entity)
		{
			try
			{
				entity.EntityId = SessionFacade.UsuarioLogado.Usuario.ent_id;
				entity = entity.Id > 0 ? modelTestBusiness.Update(entity) : modelTestBusiness.Save(entity);
			}
			catch (Exception ex)
			{
				entity.Validate.IsValid = false;
				entity.Validate.Type = ValidateType.error.ToString();
				entity.Validate.Message = string.Format("Erro ao {0} o modelo de prova.", entity.Id > 0 ? "alterar" : "salvar");

				LogFacade.SaveError(ex);
			}

			return Json(new {id = entity.Id, success = entity.Validate.IsValid, type = entity.Validate.Type, message = entity.Validate.Message }, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		public JsonResult Delete(int Id)
		{
			ModelTest entity = new ModelTest();
			try
			{
				entity = modelTestBusiness.Delete(Id, SessionFacade.UsuarioLogado.Usuario.ent_id);
			}
			catch (Exception ex)
			{
				entity.Validate.IsValid = false;
				entity.Validate.Type = ValidateType.error.ToString();
				entity.Validate.Message = "Erro ao tentar excluir o modelo de prova.";
				LogFacade.SaveError(ex);
			}

			return Json(new { success = entity.Validate.IsValid, type = entity.Validate.Type, message = entity.Validate.Message }, JsonRequestBehavior.AllowGet);
		}
		#endregion
	}
}