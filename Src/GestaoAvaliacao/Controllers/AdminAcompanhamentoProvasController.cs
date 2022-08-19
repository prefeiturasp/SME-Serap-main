using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.WebProject.Entities;
using GestaoAvaliacao.WebProject.Facade;
using System;
using System.Web.Configuration;
using System.Web.Mvc;

namespace GestaoAvaliacao.Controllers
{
    public class AdminAcompanhamentoProvasController : Controller
    {
        private readonly IAdminAcompanhamentoProvaBusiness adminAcompanhamentoProvaBusiness;

        public AdminAcompanhamentoProvasController(IAdminAcompanhamentoProvaBusiness adminAcompanhamentoProvaBusiness)
        {
            this.adminAcompanhamentoProvaBusiness = adminAcompanhamentoProvaBusiness;
        }

        [Authorize]
        public ActionResult Index()
        {
            try
            {
                if (!SessionFacade.UsuarioLogadoIsValid)
                {
                    return RedirectToAction("Index", "Error");
                };

                UsuarioLogado user = SessionFacade.UsuarioLogado;
                
                
                var resposta = adminAcompanhamentoProvaBusiness.AdminAutenticacao(new Entities.DTO.SerapEstudantes.AdminAutenticacaoDTO(user.Usuario.usu_login, user.Grupo.gru_id));

                string urlApiAcompanhamentoProva = WebConfigurationManager.AppSettings["URL_ADMIN_ACOMPANHAMENTO_PROVA"];
                if (string.IsNullOrWhiteSpace(urlApiAcompanhamentoProva))
                    throw new ApplicationException($"Necessário configurar a chave 'URL_ADMIN_ACOMPANHAMENTO_PROVA' no Web.config");

                string urlAdminAcompanhamento = $"{urlApiAcompanhamentoProva}{resposta.Codigo}";

                return Redirect(urlAdminAcompanhamento);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return RedirectToAction("Index", "Error");
            }
        }
    }
}
