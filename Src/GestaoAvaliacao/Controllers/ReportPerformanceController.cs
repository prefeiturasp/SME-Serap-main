using GestaoAvaliacao.App_Start;
using System.Web.Mvc;

namespace GestaoAvaliacao.Controllers
{
	[Authorize]
	[AuthorizeModule]
	public class ReportPerformanceController : Controller
	{
		// GET: ReportPerformance
		public ActionResult Index()
		{
			return View();
		}
	}
}