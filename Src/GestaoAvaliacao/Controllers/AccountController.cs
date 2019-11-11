using GestaoAvaliacao.WebProject;
using GestaoAvaliacao.WebProject.Facade;
using MSTech.CoreSSO.BLL;
using MSTech.CoreSSO.Entities;
using MSTech.SAML20;
using MSTech.SAML20.Bindings;
using MSTech.SAML20.Configuration;
using MSTech.SAML20.Schemas.Core;
using MSTech.SAML20.Schemas.Protocol;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Xml;

namespace GestaoAvaliacao.Controllers
{
    public class AccountController : Controller
    {
        private RedirectToRouteResult RedirectToHome { get { return RedirectToAction("Index", "Home"); } }

        private RedirectResult RedirectEndSso { get { return Redirect("/Account/LoginUsuarioGrupoSso"); } }

        #region LogIn UsuarioGrupo

        [Authorize]
        public ActionResult LoginSSO()
        {
            try
            {
                if (SessionFacade.UsuarioLogadoIsValid)
                {
                    // Carrega os grupos do usuário logado.
                    IList<SYS_Grupo> listGrupos = SYS_GrupoBO.GetSelectBySis_idAndUsu_id(SessionFacade.UsuarioLogado.Usuario.usu_id, Util.Constants.IdSistema);
                    if (listGrupos.Count > 0)
                    {
                        // Verifica se o usuário logado possui um único grupo, caso possuir não será necessário selecionar um grupo.
                        if (listGrupos.Count.Equals(1))
                        {
                            SYS_Grupo grupo = listGrupos.FirstOrDefault();
                            if (!SessionFacade.UsuarioGrupoLogadoIsValid || SessionFacade.UsuarioLogado.Grupo.gru_id != grupo.gru_id)
                            {
                                // Completa a configuração da sessão do usuário com o grupo.
                                SessionFacade.SetUsuarioGrupoLogado(grupo);

                                // Completa a autenticação com o grupo.
                                //SYS_UsuarioBO.AutenticarUsuario(SessionFacade.UsuarioLogado.Usuario, SessionFacade.UsuarioLogado.Grupo);
                                System.Web.HttpContext.Current.User.Identity.AddGrupoId(System.Web.HttpContext.Current.Request, SessionFacade.UsuarioLogado.Grupo.gru_id.ToString());

                                LogFacade.SaveSystemLog(LOG_SistemaTipo.Login, String.Format("Autenticação do usuário ({0}) com grupo ({1}) no sistema ({2}).", SessionFacade.UsuarioLogado.Usuario.usu_login, SessionFacade.UsuarioLogado.Grupo.gru_nome, ApplicationFacade.TituloSistema));
                            }
                            return RedirectToHome;
                        }
                        else
                        {
                            return View("Index", listGrupos);
                        }
                    }
                    else
                    {
                        LogFacade.SaveError(null, "Não foi possível atender a solicitação, nenhum grupo de usuário encontrado.");
                        ModelState.AddModelError(String.Empty, "Não foi possível atender a solicitação, nenhum grupo de usuário encontrado. Clique no botão voltar e tente novamente.");
                        return View("Index");
                    }
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
            catch (Exception ex)
            {
                if (!(ex is ThreadAbortException))
                {
                    LogFacade.SaveError(ex);
                    ModelState.AddModelError(String.Empty, "Não foi possível atender a solicitação. Clique no botão voltar e tente novamente.");
                    return View("Index");
                }
                else
                {
                    HttpContext.Response.RedirectPermanent("/Home/Index", false);
                    HttpContext.ApplicationInstance.CompleteRequest();

                    return View("Index");
                }
            }
        }

        /// <summary>
        /// Selecionar o um grupo
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        public ActionResult LoginUsuarioGrupo(string id)
        {
            try
            {
                if (SessionFacade.UsuarioLogadoIsValid)
                {
                    // Completa a configuração da sessão do usuário com o grupo.
                    SYS_Grupo grupo = (SYS_GrupoBO.GetEntity(new SYS_Grupo { gru_id = new Guid(id) }));
                    if (!SessionFacade.UsuarioGrupoLogadoIsValid || SessionFacade.UsuarioLogado.Grupo.gru_id != grupo.gru_id)
                    {
                        SessionFacade.SetUsuarioGrupoLogado(grupo);

                        // Completa a autenticação via FormsAuthentication com o grupo.
                        //SYS_UsuarioBO.AutenticarUsuario(SessionFacade.UsuarioLogado.Usuario, SessionFacade.UsuarioLogado.Grupo);
                        System.Web.HttpContext.Current.User.Identity.AddGrupoId(System.Web.HttpContext.Current.Request, SessionFacade.UsuarioLogado.Grupo.gru_id.ToString());

                        LogFacade.SaveSystemLog(LOG_SistemaTipo.Login, String.Format("Autenticação do usuário ({0}) com grupo ({1}) no sistema ({2}).", SessionFacade.UsuarioLogado.Usuario.usu_login, SessionFacade.UsuarioLogado.Grupo.gru_nome, ApplicationFacade.TituloSistema));
                    }
                    return RedirectToHome;
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
            catch (Exception ex)
            {
                if (!(ex is ThreadAbortException))
                {
                    LogFacade.SaveError(ex);
                    ModelState.AddModelError(String.Empty, "Não foi possível atender a solicitação. Clique no botão voltar e tente novamente.");
                    return View("Index");
                }
                else
                {
                    HttpContext.Response.RedirectPermanent("/Home/Index", false);
                    HttpContext.ApplicationInstance.CompleteRequest();

                    return View("Index");
                }
            }
        }

        public ActionResult LogoutSSO()
        {
            try
            {
                //Request.GetOwinContext().Authentication.SignOut(new string[] { "Cookies", "oidc" });
                //Request.GetOwinContext().Authentication.SignOut("Cookies");
                //Request.GetOwinContext().Authentication.SignOut("oidc");
                //Request.GetOwinContext().Authentication.SignOut("CC7970D4-5AA9-492C-A64C-3FCFABCD7371");
                Request.GetOwinContext().Authentication.SignOut();
                Session.Abandon();
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
            }
            return Redirect("/");
        }


        #endregion

        [ChildActionOnly]
        public string UrlLogout()
        {
            return "/Account/LogoutSSO";
        }

        [ChildActionOnly]
        public string UrlSistemaSso()
        {
            return String.Concat(ApplicationFacade.UrlCoreSso, "/Sistema.aspx");

            //if (Session != null)
            //{
            //    Session.Abandon();
            //}
            //Response.Redirect("~/Account/LoginSSO");
        }

        /*
         #region Properties SSO

         //private RedirectResult RedirectEndSso { get { return Redirect("/Account/LoginUsuarioGrupoSso"); } }
         //private RedirectResult RedirectEndSso { get { return Redirect("/Account/LoginSSO"); } }

         private ResponseType SamlResponse { get; set; }
         private LogoutRequestType SamlRequestLogout { get; set; }
         private SAMLAuthnRequest SamlRequestLogin { get; set; }

         #endregion

         #region LogIn SSO

         [HttpGet, HandleError(View = "Error")]
         public void LoginSso()
         {
             try
             {
                 // Recupera as configurações do ServiceProvider.
                 ServiceProvider config = ServiceProvider.GetConfig();
                 ServiceProviderEndpoint spend = SAMLUtility.GetServiceProviderEndpoint(config.ServiceEndpoint, SAMLTypeSSO.signon);

                 // Valida a configuração do ServiceProvider para sign-on no SSO.
                 if (spend == null)
                 {
                     LogFacade.SaveError(null, "Não foi possível encontrar a configuração do ServiceProvider para sign-on no SSO.");
                     System.Web.HttpContext.Current.Response.Redirect("/Error", false);
                     System.Web.HttpContext.Current.ApplicationInstance.CompleteRequest();
                 }
                 else
                 {
                     // Verifica se o usuário está autenticado, caso não envia um Resquest solicitando autenticação ao SSO.
                     if (!SessionFacade.UsuarioLogadoIsValid)
                     {
                         // Request para login.
                         SamlRequestLogin = new SAMLAuthnRequest();
                         SamlRequestLogin.Issuer = config.id;
                         SamlRequestLogin.AssertionConsumerServiceURL = Request.Url.AbsoluteUri;

                         HttpRedirectBinding binding = new HttpRedirectBinding(SamlRequestLogin, spend.localpath);
                         binding.SendRequest(System.Web.HttpContext.Current, spend.redirectUrl);
                     }
                     else
                     {
                         System.Web.HttpContext.Current.Response.Redirect(RedirectEndSso.Url, false);
                         System.Web.HttpContext.Current.ApplicationInstance.CompleteRequest();
                     }
                 }
             }
             catch (Exception ex)
             {
                 LogFacade.SaveError(ex);
             }
         }

         [HttpPost, ActionName("LoginSso"), HandleError(View = "Error")]
         public RedirectResult LoginResponseSso()
         {
             try
             {
                 if (!String.IsNullOrEmpty(Request.Unvalidated[HttpBindingConstants.SAMLResponse]))
                 {
                     // Recupera o Response.
                     string samlresponse = Request.Unvalidated[HttpBindingConstants.SAMLResponse];
                     XmlDocument doc = new XmlDocument();
                     doc.PreserveWhitespace = true;
                     doc.LoadXml(samlresponse);

                     // Valida assinatura do Response.
                     if (XmlSignatureUtils.VerifySignature(doc))
                     {
                         SamlResponse = SAMLUtility.DeserializeFromXmlString<ResponseType>(doc.InnerXml);

                         foreach (AssertionType assertion in SamlResponse.Items.Where(p => p is AssertionType))
                         {
                             NameIDType nameID = (NameIDType)assertion.Subject.Items.Where(p => p is NameIDType).FirstOrDefault();
                             if (nameID != null)
                             {
                                 FormsAuthentication.Initialize();
                                 FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(
                                     1
                                     , nameID.Value
                                     , Convert.ToDateTime(assertion.Conditions.NotBefore).ToUniversalTime()
                                     , Convert.ToDateTime(assertion.Conditions.NotOnOrAfter).ToUniversalTime()
                                     , false
                                     , String.Empty
                                     , FormsAuthentication.FormsCookiePath);

                                 HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, FormsAuthentication.Encrypt(ticket));
                                 System.Web.HttpContext.Current.Response.Cookies.Add(cookie);
                             }
                         }
                     }
                 }
             }
             catch (Exception ex)
             {
                 LogFacade.SaveError(ex);
             }

             return RedirectEndSso;
         }

         #endregion

         #region LogOut SSO

         [HttpGet, HandleError(View = "Error")]
         public void LogoutSSO()
         {
             try
             {
                 // Carrega as configurações do ServiceProvider.
                 ServiceProvider config = ServiceProvider.GetConfig();
                 ServiceProviderEndpoint spend = SAMLUtility.GetServiceProviderEndpoint(config.ServiceEndpoint, SAMLTypeSSO.logout);

                 // Valida a configuração do ServiceProvider para logout no SSO.
                 if (spend == null)
                 {
                     LogFacade.SaveError(null, "Não foi possível encontrar a configuração do ServiceProvider para logout no SSO.");
                     System.Web.HttpContext.Current.Response.Redirect("/Error", false);
                     System.Web.HttpContext.Current.ApplicationInstance.CompleteRequest();
                 }
                 else
                 {
                     // Request para logout.
                     SamlRequestLogout = new LogoutRequestType();
                     SamlRequestLogout.ID = SAMLUtility.GenerateID();
                     SamlRequestLogout.Version = SAMLUtility.VERSION;
                     SamlRequestLogout.IssueInstant = DateTime.UtcNow.AddMinutes(10);
                     SamlRequestLogout.SessionIndex = new string[] { Session.SessionID };

                     NameIDType nameID = new NameIDType();
                     nameID.Format = SAMLUtility.NameIdentifierFormats.Transient;
                     nameID.Value = spend.localpath;

                     SamlRequestLogout.Item = nameID;
                     SamlRequestLogout.Issuer = new NameIDType();
                     SamlRequestLogout.Issuer.Value = config.id;

                     MemoryStream ms = new MemoryStream();
                     using (StreamWriter writer = new StreamWriter(new DeflateStream(ms, CompressionMode.Compress, true), Encoding.GetEncoding("iso-8859-1")))
                     {
                         writer.Write(SAMLUtility.SerializeToXmlString(SamlRequestLogout));
                         writer.Close();
                     }
                     string message = HttpUtility.UrlEncode(Convert.ToBase64String(ms.GetBuffer(), 0, (int)ms.Length, Base64FormattingOptions.None));
                     HttpRedirectBinding binding = new HttpRedirectBinding(message, spend.localpath);
                     binding.SendRequest(System.Web.HttpContext.Current, spend.redirectUrl);
                 }
             }
             catch (Exception ex)
             {
                 LogFacade.SaveError(ex);
             }

         }

         [HttpPost, ActionName("LogoutSSO"), HandleError(View = "Error")]
         public RedirectResult LogoutResponseSso()
         {
             string redirectUrl = RedirectEndSso.Url;

             try
             {
                 // Carrega as configurações do ServiceProvider.
                 ServiceProvider config = ServiceProvider.GetConfig();
                 ServiceProviderEndpoint spend = SAMLUtility.GetServiceProviderEndpoint(config.ServiceEndpoint, SAMLTypeSSO.logout);

                 // Valida a configuração do ServiceProvider para logout no SSO.
                 if (spend == null)
                 {
                     LogFacade.SaveError(null, "Não foi possível encontrar a configuração do ServiceProvider para logout no SSO.");
                     redirectUrl = "/Error";
                 }
                 else
                 {
                     if (!String.IsNullOrEmpty(Request.Unvalidated[HttpBindingConstants.SAMLResponse]))
                     {
                         // Recupera o Response.
                         string samlresponse = Request.Unvalidated[HttpBindingConstants.SAMLResponse];
                         XmlDocument doc = new XmlDocument();
                         doc.PreserveWhitespace = true;
                         doc.LoadXml(samlresponse);
                         SamlResponse = SAMLUtility.DeserializeFromXmlString<ResponseType>(doc.InnerXml);

                         FormsAuthentication.SignOut();
                         if (Session != null)
                         {
                             Session.Abandon();
                         }
                     }

                     redirectUrl = spend.redirectUrl;
                 }
             }
             catch (Exception ex)
             {
                 LogFacade.SaveError(ex);
             }

             return Redirect(redirectUrl);
         }

         #endregion

         #region LogIn UsuarioGrupo

         [Authorize]
         public ActionResult LoginUsuarioGrupoSso()
         {
             try
             {
                 if (SessionFacade.UsuarioLogadoIsValid)
                 {
                     // Carrega os grupos do usuário logado.
                     IList<SYS_Grupo> listGrupos = SYS_GrupoBO.GetSelectBySis_idAndUsu_id(SessionFacade.UsuarioLogado.Usuario.usu_id, Util.Constants.IdSistema);
                     if (listGrupos.Count > 0)
                     {
                         // Verifica se o usuário logado possui um único grupo, caso possuir não será necessário selecionar um grupo.
                         if (listGrupos.Count.Equals(1))
                         {
                             SYS_Grupo grupo = listGrupos.FirstOrDefault();

                             if (!SessionFacade.UsuarioGrupoLogadoIsValid || SessionFacade.UsuarioLogado.Grupo.gru_id != grupo.gru_id)
                             {
                                 // Completa a configuração da sessão do usuário com o grupo.
                                 SessionFacade.SetUsuarioGrupoLogado(grupo);

                                 // Completa a autenticação via FormsAuthentication com o grupo.
                                 SYS_UsuarioBO.AutenticarUsuario(SessionFacade.UsuarioLogado.Usuario, SessionFacade.UsuarioLogado.Grupo);
                                 LogFacade.SaveSystemLog(LOG_SistemaTipo.Login, String.Format("Autenticação do usuário ({0}) com grupo ({1}) no sistema ({2}).", SessionFacade.UsuarioLogado.Usuario.usu_login, SessionFacade.UsuarioLogado.Grupo.gru_nome, ApplicationFacade.TituloSistema));
                             }

                             return RedirectToHome;
                         }
                         else
                         {
                             return View("Index", listGrupos);
                         }
                     }
                     else
                     {
                         LogFacade.SaveError(null, "Não foi possível atender a solicitação, nenhum grupo de usuário encontrado.");
                         ModelState.AddModelError(String.Empty, "Não foi possível atender a solicitação, nenhum grupo de usuário encontrado. Clique no botão voltar e tente novamente.");
                         return View("Index");
                     }
                 }
                 else
                 {
                     throw new NotImplementedException();
                 }
             }
             catch (Exception ex)
             {
                 if (!(ex is ThreadAbortException))
                 {
                     LogFacade.SaveError(ex);
                     ModelState.AddModelError(String.Empty, "Não foi possível atender a solicitação. Clique no botão voltar e tente novamente.");
                     return View("Index");
                 }
                 else
                 {
                     HttpContext.Response.RedirectPermanent("/Home/Index", false);
                     HttpContext.ApplicationInstance.CompleteRequest();

                     return View("Index");
                 }
             }
         }

         [Authorize]
         public ActionResult LoginUsuarioGrupo(string id)
         {
             try
             {
                 if (SessionFacade.UsuarioLogadoIsValid)
                 {
                     // Completa a configuração da sessão do usuário com o grupo.
                     SYS_Grupo grupo = (SYS_GrupoBO.GetEntity(new SYS_Grupo { gru_id = new Guid(id) }));

                     if (!SessionFacade.UsuarioGrupoLogadoIsValid || SessionFacade.UsuarioLogado.Grupo.gru_id != grupo.gru_id)
                     {
                         SessionFacade.SetUsuarioGrupoLogado(grupo);

                         // Completa a autenticação via FormsAuthentication com o grupo.
                         SYS_UsuarioBO.AutenticarUsuario(SessionFacade.UsuarioLogado.Usuario, SessionFacade.UsuarioLogado.Grupo);
                         LogFacade.SaveSystemLog(LOG_SistemaTipo.Login, String.Format("Autenticação do usuário ({0}) com grupo ({1}) no sistema ({2}).", SessionFacade.UsuarioLogado.Usuario.usu_login, SessionFacade.UsuarioLogado.Grupo.gru_nome, ApplicationFacade.TituloSistema));
                     }

                     return RedirectToHome;
                 }
                 else
                 {
                     throw new NotImplementedException();
                 }
             }
             catch (Exception ex)
             {
                 if (!(ex is ThreadAbortException))
                 {
                     LogFacade.SaveError(ex);
                     ModelState.AddModelError(String.Empty, "Não foi possível atender a solicitação. Clique no botão voltar e tente novamente.");
                     return View("Index");
                 }
                 else
                 {
                     HttpContext.Response.RedirectPermanent("/Home/Index", false);
                     HttpContext.ApplicationInstance.CompleteRequest();

                     return View("Index");
                 }
             }
         }

         #endregion

         [ChildActionOnly]
         public string UrlLogout()
         {
             return "/Account/LogoutSSO";
         }

         [ChildActionOnly]
         public string UrlSistemaSso()
         {
             return String.Concat(ApplicationFacade.UrlCoreSso, "/Sistema.aspx");
         }

     */

    }
}