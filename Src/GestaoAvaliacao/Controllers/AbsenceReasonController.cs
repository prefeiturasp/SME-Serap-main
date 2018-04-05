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
    [Authorize]
	[AuthorizeModule]
	public class AbsenceReasonController : Controller
	{
		private readonly IAbsenceReasonBusiness absenceReasonBusiness;

		public AbsenceReasonController(IAbsenceReasonBusiness absenceReasonBusiness)
		{
			this.absenceReasonBusiness = absenceReasonBusiness;
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
		public JsonResult Find(int Id)
		{
			AbsenceReason absenceReason = absenceReasonBusiness.Get(Id, SessionFacade.UsuarioLogado.Usuario.ent_id);
			return Json(new { success = true, absenceReason = absenceReason }, JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		public JsonResult FindSimple(int Id)
		{
			try
			{
				AbsenceReason absenceReason = absenceReasonBusiness.Get(Id, SessionFacade.UsuarioLogado.Usuario.ent_id);

				var retorno = new
				{
                    IsDefault = absenceReason.IsDefault,
                    AllowRetry = absenceReason.AllowRetry,
					Description = absenceReason.Description,
					Id = absenceReason.Id
				};

				return Json(new { success = true, absenceReason = retorno }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				LogFacade.SaveError(ex);
				return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao carregar o motivo de ausência para edição." }, JsonRequestBehavior.AllowGet);
			}
		}

		[HttpGet]
		public JsonResult LoadCombo()
		{
			try
			{
				var absenceReason = absenceReasonBusiness.Get(SessionFacade.UsuarioLogado.Usuario.ent_id);

				var retorno = absenceReason.Select(a => new
				{
					Description = a.Description,
					Id = a.Id
				});

				return Json(new { success = true, lista = retorno }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				LogFacade.SaveError(ex);
				return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao carregar o motivo de ausência para edição." }, JsonRequestBehavior.AllowGet);
			}
		}

		[HttpGet]
		[Paginate]
		public JsonResult Load()
		{
			try
			{
				Pager pager = this.GetPager();
				IEnumerable<AbsenceReason> lista = absenceReasonBusiness.Load(ref pager, SessionFacade.UsuarioLogado.Usuario.ent_id);

				if (lista != null && lista.Count() > 0)
				{
					var absenceReasonList = lista.Select(x => new
					{
						Id = x.Id,
						Description = x.Description,
                        IsDefault = x.IsDefault
                    });

					return Json(new { success = true, lista = absenceReasonList }, JsonRequestBehavior.AllowGet);
				}
				else
					return Json(new { success = true, lista }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				LogFacade.SaveError(ex);
				return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar motivo de ausência pesquisado." }, JsonRequestBehavior.AllowGet);
			}
		}

		[HttpGet]
		[Paginate]
		public JsonResult Search(String search)
		{
			try
			{
				Pager pager = this.GetPager();
				IEnumerable<AbsenceReason> lista = search != null ? absenceReasonBusiness.Search(search, ref pager, SessionFacade.UsuarioLogado.Usuario.ent_id) : absenceReasonBusiness.Load(ref pager, SessionFacade.UsuarioLogado.Usuario.ent_id);

				if (lista != null && lista.Count() > 0)
				{
					var absenceReasonList = lista.Select(x => new
					{
						Id = x.Id,
						Description = x.Description,
                        IsDefault = x.IsDefault
                    });

					return Json(new { success = true, lista = absenceReasonList }, JsonRequestBehavior.AllowGet);
				}
				else
					return Json(new { success = true, lista }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				LogFacade.SaveError(ex);
				return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar motivo de ausência pesquisado." }, JsonRequestBehavior.AllowGet);
			}
		}

		#endregion

		#region Write

		[HttpPost]
		public JsonResult Save(AbsenceReason entity)
		{
			try
			{
				if (entity.Id > 0)
				{
					entity = absenceReasonBusiness.Update(entity.Id, entity, SessionFacade.UsuarioLogado.Usuario.ent_id);
				}
				else
				{
					entity = absenceReasonBusiness.Save(entity, SessionFacade.UsuarioLogado.Usuario.ent_id);
				}
			}
			catch (Exception ex)
			{
				entity.Validate.IsValid = false;
				entity.Validate.Type = ValidateType.error.ToString();
				entity.Validate.Message = string.Format("Erro ao {0} o motivo de ausência do item.", entity.Id > 0 ? "alterar" : "salvar");

				LogFacade.SaveError(ex);
			}

			return Json(new { success = entity.Validate.IsValid, type = entity.Validate.Type, message = entity.Validate.Message }, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		public JsonResult Delete(int Id)
		{
			AbsenceReason entity = new AbsenceReason();

			try
			{
				entity = absenceReasonBusiness.Delete(Id, SessionFacade.UsuarioLogado.Usuario.ent_id);
			}
			catch (Exception ex)
			{
				entity.Validate.IsValid = false;
				entity.Validate.Type = ValidateType.error.ToString();
				entity.Validate.Message = "Erro ao tentar excluir o motivo de ausência.";
				LogFacade.SaveError(ex);
			}

			return Json(new { success = entity.Validate.IsValid, type = entity.Validate.Type, message = entity.Validate.Message }, JsonRequestBehavior.AllowGet);
		}

		#endregion
	}
}