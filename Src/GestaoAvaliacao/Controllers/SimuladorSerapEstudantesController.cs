using GestaoAvaliacao.WebProject.Facade;
using System.Web.Configuration;
using System;
using System.Web.Mvc;
using GestaoAvaliacao.Entities.DTO.SerapEstudantes;
using GestaoAvaliacao.IBusiness;

namespace GestaoAvaliacao.Controllers
{
    public class SimuladorSerapEstudantesController : Controller
    {
        private readonly ISerapEstudantesBusiness serapEstudantesBusiness;

        public SimuladorSerapEstudantesController(ISerapEstudantesBusiness serapEstudantesBusiness)
        {
            this.serapEstudantesBusiness = serapEstudantesBusiness;
        }

        [Authorize]
        public ActionResult Index(long blockId)
        {
            try
            {
                if (!SessionFacade.UsuarioLogadoIsValid)
                    throw new NotImplementedException();

                var user = SessionFacade.UsuarioLogado;
                var resposta = serapEstudantesBusiness.SimuladorAutenticacao(new SimuladorAutenticacaoDTO(user.Usuario.usu_login, user.Grupo.gru_id));

                if (string.IsNullOrEmpty(resposta.Codigo))
                    throw new ApplicationException("Usuário não autorizado.");

                var urlSimuladorSerapEstudantes = WebConfigurationManager.AppSettings["URL_SIMULADOR_SERAP_ESTUDANTES"];

                if (string.IsNullOrWhiteSpace(urlSimuladorSerapEstudantes))
                    throw new ApplicationException("Necessário configurar a chave 'URL_SIMULADOR_SERAP_ESTUDANTES' no Web.config");

                if (blockId <= 0)
                    throw new ApplicationException("É necessário informar o ID do caderno.");

                var urlSimulador = $"{urlSimuladorSerapEstudantes}Prova/caderno/{blockId}";

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