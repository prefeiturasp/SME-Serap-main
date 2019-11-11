using global::GestaoAvaliacao.WebProject.Facade;
using System.Web.Mvc;

namespace GestaoAvaliacao.App_Start
{
    public class AuthorizeModule : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            if (filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                if (!SessionFacade.UsuarioGrupoLogadoIsValid)
                {
                    filterContext.HttpContext.Response.Redirect(IdentitySettingsConfig.IDSSettings.RedirectUri);
                }

                if (!filterContext.IsChildAction && SessionFacade.UsuarioGrupoLogadoIsValid)
                {
                    SessionFacade.SetAuthorizeModule(filterContext.HttpContext.Request.RequestContext.RouteData.Values["Controller"].ToString());

                    // Caso tenha grupo permissão, verifica as permissões no módulo atual para o usuário. Verifica permissão do usuário, caso não tenha nehuma permissão na página redireciona para a Index.
                    if (SessionFacade.CurrentGrupoPermissaoIsValid
                        && ((!SessionFacade.CurrentGrupoPermissao.grp_consultar) && (!SessionFacade.CurrentGrupoPermissao.grp_inserir) &&
                            (!SessionFacade.CurrentGrupoPermissao.grp_alterar) && (!SessionFacade.CurrentGrupoPermissao.grp_excluir)))
                    {
                        System.Web.HttpContext.Current.Response.Redirect("/", false);
                        System.Web.HttpContext.Current.ApplicationInstance.CompleteRequest();
                    }
                }
           }
        }
    }
}