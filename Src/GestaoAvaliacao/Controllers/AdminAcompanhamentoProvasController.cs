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

                if (resposta.StatusCode == 200)
                {
                    string urlApiAcompanhamentoProva = WebConfigurationManager.AppSettings["URL_ADMIN_ACOMPANHAMENTO_PROVA"];
                    if (string.IsNullOrWhiteSpace(urlApiAcompanhamentoProva))
                        throw new ApplicationException($"Necessário configurar a chave 'URL_ADMIN_ACOMPANHAMENTO_PROVA' no Web.config");

                    string urlAdminAcompanhamento = $"{urlApiAcompanhamentoProva}{resposta.Codigo}";

                    return Redirect(urlAdminAcompanhamento);
                }
                else
                {
                    string mensagem = $"Usuário '{user.Usuario.usu_login}' com o grupo '{user.Grupo.gru_nome}' não possui permissão para acessar esta funcionalidade.";
                    return RedirectToAction("Index", "Error", new { mensagem });
                }
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);

                if(ex.InnerException != null)
                    LogFacade.SaveError(ex.InnerException);

                return RedirectToAction("Index", "Error");
            }
        }
    }
}
