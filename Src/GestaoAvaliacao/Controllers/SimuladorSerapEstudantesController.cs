using GestaoAvaliacao.WebProject.Facade;
using System.Web.Configuration;
using System;
using System.Web.Mvc;

namespace GestaoAvaliacao.Controllers
{
    public class SimuladorSerapEstudantesController : Controller
    {
        [Authorize]
        public ActionResult Index(long blockId)
        {
            try
            {
                if (!SessionFacade.UsuarioLogadoIsValid)
                    throw new NotImplementedException();

                var urlSimuladorSerapEstudantes = WebConfigurationManager.AppSettings["URL_SIMULADOR_SERAP_ESTUDANTES"];

                if (string.IsNullOrWhiteSpace(urlSimuladorSerapEstudantes))
                    throw new ApplicationException("Necessário configurar a chave 'URL_SIMULADOR_SERAP_ESTUDANTES' no Web.config");

                if (blockId <= 0)
                    throw new ApplicationException("É necessário informar o ID do caderno.");

                var urlSimulador = $"{urlSimuladorSerapEstudantes}{blockId}";

                return Redirect(urlSimulador);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);

                if (ex.InnerException != null)
                    LogFacade.SaveError(ex.InnerException);

                return RedirectToAction("Index", "Error");
            }
        }
    }
}