using GestaoAvaliacao.API.App_Start;
using System.Web.Mvc;

namespace GestaoAvaliacao.API.Controllers
{
    [CustomAuthorizationAttribute]
    public class DocsController : Controller
    {
        public ActionResult AnswerSheet()
        {
            return View();
        }
	}
}