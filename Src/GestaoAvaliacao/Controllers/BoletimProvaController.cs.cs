using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.WebProject.Entities;
using GestaoAvaliacao.WebProject.Facade;
using System;
using System.Web.Configuration;
using System.Web.Mvc;

namespace GestaoAvaliacao.Controllers
{
    public class BoletimProvasController : Controller
    {
        private readonly IBoletimProvaBusiness boletimProvaBusiness;

        public BoletimProvasController(IBoletimProvaBusiness boletimProvaBusiness)
        {
            this.boletimProvaBusiness = boletimProvaBusiness;
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


                var resposta = boletimProvaBusiness.AdminAutenticacao(new Entities.DTO.SerapEstudantes.AdminAutenticacaoDTO(user.Usuario.usu_login, user.Grupo.gru_id));

                string urlApiBoletimProva = WebConfigurationManager.AppSettings["URL_BOLETIM_PROVA"];
                if (string.IsNullOrWhiteSpace(urlApiBoletimProva))
                    throw new ApplicationException($"Necessário configurar a chave 'URL_BOLETIM_PROVA' no Web.config");

                string urlAdminAcompanhamento = $"{urlApiBoletimProva}{resposta.Codigo}";

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