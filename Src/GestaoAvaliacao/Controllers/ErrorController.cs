using System.Web.Mvc;

namespace GestaoAvaliacao.Controllers
{
    public class ErrorController : Controller
    {
        public ActionResult Index(string mensagem = null)
        {
            if(string.IsNullOrEmpty(mensagem))
                ViewBag.Message = "Ocorreu um erro inesperado. Por favor, tente novamente. Se o problema persistir, avise a equipe de suporte e apoio da ferramenta.";
            else
                ViewBag.Message = mensagem;

            return View();
        }
	}
}