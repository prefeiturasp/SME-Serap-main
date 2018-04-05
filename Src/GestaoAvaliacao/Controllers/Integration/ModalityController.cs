using GestaoAvaliacao.Util;
using GestaoAvaliacao.WebProject.Facade;
using GestaoEscolar.Entities;
using GestaoEscolar.IBusiness;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace GestaoAvaliacao.Controllers.Integration
{
    [Authorize]
	public class ModalityController : Controller
	{
		private readonly IACA_TipoModalidadeEnsinoBusiness modalityBusiness;

		public ModalityController(IACA_TipoModalidadeEnsinoBusiness modalityBusiness)
		{
			this.modalityBusiness = modalityBusiness;
		}

		public ActionResult Index()
		{
			return View();
		}

		#region Read

		[HttpGet]
		public JsonResult Load()
		{
			try
			{
				IEnumerable<ACA_TipoModalidadeEnsino> lista = modalityBusiness.Load();

				if (lista != null && lista.Count() > 0)
				{
					var modalityList = lista.Select(x => new
					{
						Id = x.tme_id,
						Description = x.tme_nome,
					});

					return Json(new { success = true, lista = modalityList }, JsonRequestBehavior.AllowGet);
				}
				else
					return Json(new { success = false, type = ValidateType.alert.ToString(), message = "Modalidade não encontrada." }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				LogFacade.SaveError(ex);
				return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao carregar modalidade." }, JsonRequestBehavior.AllowGet);
			}
		}

		#endregion
	}
}