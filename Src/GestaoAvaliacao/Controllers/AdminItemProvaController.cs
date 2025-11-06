using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.WebProject.Entities;
using GestaoAvaliacao.WebProject.Facade;
using System;
using System.Web.Configuration;
using System.Web.Mvc;


namespace GestaoAvaliacao.Controllers
{
    public class AdminItemProvaController : Controller
    {
        private readonly IAdminItemProvaBusiness _adminItemProvaBusiness;

        public AdminItemProvaController(IAdminItemProvaBusiness adminItemProvaBusiness)
        {
            this._adminItemProvaBusiness = adminItemProvaBusiness;
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


                var resposta = _adminItemProvaBusiness.AdminAutenticacao(new Entities.DTO.SerapEstudantes.AdminAutenticacaoDTO(user.Usuario.usu_login, user.Grupo.gru_id));

                string urlApiItemProva = WebConfigurationManager.AppSettings["URL_ADMIN_ITEM_PROVA"];
                if (string.IsNullOrWhiteSpace(urlApiItemProva))
                    throw new ApplicationException($"Necessário configurar a chave 'URL_ADMIN_ITEM_PROVA' no Web.config");

                string urlItemProva = $"{urlApiItemProva}validar?codigo={resposta.Codigo}";

                return Redirect(urlItemProva);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return RedirectToAction("Index", "Error");
            }
        }
    }
}