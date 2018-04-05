using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.Util;
using GestaoAvaliacao.WebProject.Facade;
using System;
using System.Linq;
using System.Web.Mvc;

namespace GestaoAvaliacao.Controllers
{
    public class TestPerformanceLevelController : Controller
	{
		private readonly ITestPerformanceLevelBusiness testPerformanceLevelBusiness;

		public TestPerformanceLevelController(ITestPerformanceLevelBusiness testPerformanceLevelBusiness)
		{
			this.testPerformanceLevelBusiness = testPerformanceLevelBusiness;
		}

		public ActionResult Index()
		{
			return View();
		}

		#region Read

		[HttpGet]
		public JsonResult GetPerformanceLevelByTest(long testId)
		{
			try
			{
				var entity = testPerformanceLevelBusiness.GetPerformanceLevelByTest(testId);

				if (entity != null)
				{
					var ret = entity.Select(p => new
					{
						Id = p.Id,
						PerformanceId = p.PerformanceLevel.Id,
						Value1 = p.Value1,
						Value2 = p.Value2
					});

					return Json(new { success = true, lista = ret }, JsonRequestBehavior.AllowGet);
				}

				return Json(new { success = false, type = ValidateType.alert.ToString(), message = "Nível de desempenho não encontrado." }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				LogFacade.SaveError(ex);
				return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar nível de desempenho pesquisado." }, JsonRequestBehavior.AllowGet);
			}
		}

		#endregion
	}
}