using GestaoAvaliacao.App_Start;
using GestaoAvaliacao.Util;
using GestaoAvaliacao.WebProject.Entities;
using GestaoAvaliacao.WebProject.Facade;
using MSTech.CoreSSO.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace GestaoAvaliacao.Controllers
{
    [Authorize]
	[AuthorizeModule]
	public class LayoutController : Controller
	{
		[ChildActionOnly]
		public PartialViewResult Sistemas()
		{
			IList<SYS_Sistema> sistemas = new List<SYS_Sistema>();
			try
			{
				sistemas = SessionFacade.UsuarioLogado.Sistemas;
			}
			catch (Exception exception)
			{
				LogFacade.SaveError(exception);
			}
			return PartialView("~/Views/Partial/Sistemas.cshtml", sistemas);
		}

		[ChildActionOnly]
		public string NomeUsuarioLogado()
		{
			string nomeUsuario = String.Empty;
			try
			{
				nomeUsuario = SessionFacade.UsuarioLogado.Nome;
			}
			catch (Exception exception)
			{
				LogFacade.SaveError(exception);
			}
			return nomeUsuario;
		}

		[ChildActionOnly]
		public string VersaoSistema()
		{
			string versaoSistema = String.Empty;
			try
			{
				versaoSistema = ApplicationFacade.Versao;
			}
			catch (Exception exception)
			{
				LogFacade.SaveError(exception);
				versaoSistema = string.Empty;
			}
			return versaoSistema;
		}

		[HttpGet]
		public JsonResult Menu()
		{
			List<Menu> lista = new List<Menu>();
			try
			{
				lista = SessionFacade.MenuUsuario;
			}
			catch (Exception exception)
			{
				LogFacade.SaveError(exception);
			}
			return Json(lista, JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		public JsonResult MenuAngular()
		{
			List<Menu> lista = new List<Menu>();
			try
			{
				lista = ApplicationFacade.GetMenu(SessionFacade.UsuarioLogado.Grupo.gru_id, SessionFacade.UsuarioLogado.Grupo.vis_id);
			}
			catch (Exception exception)
			{
				LogFacade.SaveError(exception);
			}
			return Json(new { success = true, lista = lista }, JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		public JsonResult GetSistemas()
		{
			try
			{
				var lista = SessionFacade.UsuarioLogado.Sistemas.Select(i => new { caminho = i.sis_caminho, nome = i.sis_nome }).ToList();
				return Json(new { success = true, lista = lista }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception exception)
			{
				LogFacade.SaveError(exception);
				return Json(new { success = false, type = ValidateType.error.ToString(), message = exception.Message }, JsonRequestBehavior.AllowGet);
			}
		}
	}
}