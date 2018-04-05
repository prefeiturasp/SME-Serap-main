using GestaoAvaliacao.IBusiness;
using System.Web.Mvc;

namespace GestaoAvaliacao.API.Controllers
{
    public class HomeController : Controller
	{		
		public HomeController(ICorrectionBusiness correctionBusiness, IStudentTestAbsenceReasonBusiness studentTestAbsenceReasonBusiness)
		{			
		}
		public ActionResult Index()
		{
			return View();
		}
	}
}
