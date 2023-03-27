using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.WebProject.Entities;
using GestaoAvaliacao.WebProject.Facade;
using System;
using System.Web.Configuration;
using System.Web.Mvc;

namespace GestaoAvaliacao.Controllers
{
    public class AdminSerapEstudantesController : Controller
    {
        private readonly ISerapEstudantesBusiness serapEstudantesBusiness;

        public AdminSerapEstudantesController(ISerapEstudantesBusiness serapEstudantesBusiness)
        {
            this.serapEstudantesBusiness = serapEstudantesBusiness;
        }

        [Authorize]
        public ActionResult Index()
        {
            try
            {
                if (!SessionFacade.UsuarioLogadoIsValid)
                {
                    throw new NotImplementedException();
                };

                UsuarioLogado user = SessionFacade.UsuarioLogado;
                var resposta = serapEstudantesBusiness.AdminAutenticacao(new Entities.DTO.SerapEstudantes.AdminAutenticacaoDTO(user.Usuario.usu_login, user.Grupo.gru_id));

                string urlApiSerapEstudantes = WebConfigurationManager.AppSettings["URL_ADMIN_SERAP_ESTUDANTES"];
                if (string.IsNullOrWhiteSpace(urlApiSerapEstudantes))
                    throw new ApplicationException($"Necessário configurar a chave 'URL_ADMIN_SERAP_ESTUDANTES' no Web.config");

                string urlAdminEstudantes = $"{urlApiSerapEstudantes}{resposta.Codigo}";

                return Redirect(urlAdminEstudantes);
            }
            catch(Exception ex)
            {
                LogFacade.SaveError(ex);

                if (ex.InnerException != null)
                    LogFacade.SaveError(ex.InnerException);

                return RedirectToAction("Index", "Error");
            }
        }
    }
}
