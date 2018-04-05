using GestaoAvaliacao.App_Start;
using GestaoAvaliacao.Entities;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.MongoEntities;
using GestaoAvaliacao.MongoEntities.DTO;
using GestaoAvaliacao.Util;
using GestaoAvaliacao.WebProject.Facade;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GestaoAvaliacao.Controllers
{
    public class ResponseChangeLogController : Controller
	{
		private readonly IResponseChangeLogBusiness responseChangeLogBusiness;

		public ResponseChangeLogController(IResponseChangeLogBusiness responseChangeLogBusiness)
		{
			this.responseChangeLogBusiness = responseChangeLogBusiness;
		}

		public ActionResult Index()
		{
			return View();
		}

        #region Read

        [HttpGet]
        [Paginate]
        public JsonResult GetResponseChangeLog(long test_id, Guid? uad_id, long? esc_id, long? tur_id, DateTime? DateStartChange, DateTime? DateEndChange)
        {
            try
            {
                Pager pager = this.GetPager();
                List<ResponseChangeLogDTO> lista = responseChangeLogBusiness.GetResponseChangeLog(test_id, SessionFacade.UsuarioLogado.Usuario.ent_id, uad_id, esc_id, tur_id, DateStartChange, DateEndChange, ref pager);

                return Json(new { success = true, lista }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar log de alteração de respostas pesquisado." }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

    }
}