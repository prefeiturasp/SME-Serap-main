using GestaoAvaliacao.Util;
using GestaoAvaliacao.WebProject.Entities;
using GestaoEscolar.Business;
using GestaoEscolar.Entities;
using MSTech.CoreSSO.BLL;
using MSTech.CoreSSO.Entities;
using MSTech.Validation.Exceptions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Web;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace GestaoAvaliacao.WebProject.Facade
{
    public static class SessionFacade
    {
        #region Constants

        private const string USUARIO_LOGADO = "USUARIO_LOGADO";
        private const string GRUPO_PERMISSAO = "GRUPO_PERMISSAO";

        #endregion

        #region Properties UsuarioLogado

		/// <summary>
		/// Retorna a entidade UsuarioLogado, contendo dados do usuário logado.
		/// </summary>
		public static UsuarioLogado UsuarioLogado
		{
			get
			{
                if (HttpContext.Current.User.Identity.IsAuthenticated)
                {
                    if (HttpContext.Current.Session[USUARIO_LOGADO] == null)
                    {
                        LoadUsuarioLogado();
                    }
                    return (UsuarioLogado)HttpContext.Current.Session[USUARIO_LOGADO];
                }
                return null;
            }
			private set
			{
				HttpContext.Current.Session[USUARIO_LOGADO] = value;
			}
		}

        /// <summary>
        /// Retorna se as informações do usuário logado são válidas.
        /// </summary>
        public static bool UsuarioLogadoIsValid
        {
            get
            {
                return UsuarioLogado != null;
            }
        }

        /// <summary>
        /// Retorna se as informações do grupo do usuário logado são válidas.
        /// </summary>
        public static bool UsuarioGrupoLogadoIsValid
        {
            get
            {
                return (UsuarioLogadoIsValid && UsuarioLogado.Grupo != null);
            }
        }

        public static NotificacaoConfig NotificacaoConfig
        {
            get
            {
                var api = ConfigurationManager.AppSettings["UrlNotificationApi"];
                var signalR = ConfigurationManager.AppSettings["UrlNotificationSignalR"];

                NotificacaoConfig notifConfig = new NotificacaoConfig
                {
                    UrlNotificationAPI = api,
                    UrlNotificationSignalR = signalR
                };

                var cfg = new NotificacaoConfig(notifConfig);
                var userIdentity = HttpContext.Current.User;
                cfg.UserId = UserIdentityExtension.GetUserId(userIdentity);
                cfg.IDSToken = UserIdentityExtension.GetToken(userIdentity);
                return cfg;
            }
        }

        #endregion

        #region Properties GrupoPermissao

        /// <summary>
        /// Retorna se a permissão do grupo de usuário no módulo corrente é válida.
        /// </summary>
        public static bool CurrentGrupoPermissaoIsValid
        {
            get
            {
                return (CurrentGrupoPermissao != null) && (CurrentGrupoPermissao.mod_id > 0);
            }
        }


        /// <summary>
        /// Retorna a entidade SYS_GrupoPermissao, contendo o grupo permissão acessado na requisição corrente. 
        /// </summary>
        public static SYS_GrupoPermissao CurrentGrupoPermissao
        {
            get
            {
                return (SYS_GrupoPermissao)HttpContext.Current.Session[GRUPO_PERMISSAO];
            }
            private set
            {
                HttpContext.Current.Session[GRUPO_PERMISSAO] = value;
            }
        }

        #endregion

        #region Properties Menu

        public static List<Menu> MenuUsuario
        {
            get
            {
                return ApplicationFacade.GetMenu(SessionFacade.UsuarioLogado.Grupo.gru_id, SessionFacade.UsuarioLogado.Grupo.vis_id);
            }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Configura a permissão do usuário logado no sistema de acordo com seu grupo.
        /// </summary>
        /// <param name="virtualPath">Caminho virtual da requisição</param>
        public static void SetAuthorizeModule(string virtualPath)
        {
            CurrentGrupoPermissao = ApplicationFacade.SetAuthorizeModule(UsuarioLogado.Grupo.gru_id, virtualPath);
        }

        /// <summary>
        /// Configura a permissão do usuário logado no sistema de acordo com seu grupo.
        /// </summary>
        /// <param name="virtualPath">Caminho virtual da requisição</param>
        public static SYS_GrupoPermissao GetAuthorizeModule(string virtualPath)
        {
            return ApplicationFacade.SetAuthorizeModule(UsuarioLogado.Grupo.gru_id, virtualPath);
        }

        /// <summary>
        /// Configura o grupo de permissão para o usuário logado.
        /// </summary>
        /// <param name="grupo">Entidade SYS_Grupo</param>
        public static void SetUsuarioGrupoLogado(SYS_Grupo grupo)
        {
            if (!grupo.IsNew)
            {
                // Configura a sessão do usuário com o grupo de usuário.
                UsuarioLogado.Grupo = grupo;
            }
            else
            {
                throw new ValidationException("Grupo de usuário inválido.");
            }
        }

        /// <summary>
        /// Carrega os modulos filhos à partir do modulo pai passado por parâmetro.
        /// Se o valor passado por parâmetro for zero, serão carregados os modulos raiz.
        /// </summary>
        /// <param name="moduloPaiId"></param>
        /// <returns>Retorna uma string html com os modulos</returns>
        public static string Modulo(int moduloPaiId = 0)
        {
            string menuXml = SYS_ModuloBO.CarregarSiteMapXML2(
                    UsuarioLogado.Grupo.gru_id,
                    UsuarioLogado.Grupo.sis_id,
                    UsuarioLogado.Grupo.vis_id,
                    moduloPaiId
                    );
            if (String.IsNullOrEmpty(menuXml))
            {
                menuXml = "<menus/>";
            }
            menuXml = menuXml.Replace("url=\"~/", "url=\"");

            XmlTextReader xmlTextReader = new XmlTextReader(new StringReader(menuXml));
            XPathDocument xPathDocument = new XPathDocument(xmlTextReader);
            XslCompiledTransform xslCompiledTransform = new XslCompiledTransform();
            xslCompiledTransform.Load(HttpContext.Current.Server.MapPath("~/Content/SiteMap.xslt"));

            StringWriter stringWriter = new StringWriter();
            xslCompiledTransform.Transform(xPathDocument, null, stringWriter);

            return stringWriter.ToString();
        }

        #region [ VerifyPermission ]
        public static bool IsModuleAuthorized(string virtualPath, Permission permissionRequired)
        {
            if (SessionFacade.UsuarioGrupoLogadoIsValid)
            {
                SYS_GrupoPermissao groupPermission = SessionFacade.GetAuthorizeModule(virtualPath);
                if (groupPermission != null && groupPermission.mod_id > 0)
                {
                    switch (permissionRequired)
                    {
                        case Permission.Create:
                            {
                                return groupPermission.grp_inserir;
                            }
                        case Permission.Read:
                            {
                                return groupPermission.grp_consultar;
                            }
                        case Permission.Update:
                            {
                                return groupPermission.grp_alterar;
                            }
                        case Permission.Delete:
                            {
                                return groupPermission.grp_excluir;
                            }
                        case Permission.CreateOrUpdate:
                            {
                                return groupPermission.grp_inserir
                                    || groupPermission.grp_alterar;
                            }
                        case Permission.All:
                            {
                                return groupPermission.grp_inserir
                                    && groupPermission.grp_consultar
                                    && groupPermission.grp_alterar
                                    && groupPermission.grp_excluir;
                            }
                        default:
                            return false;
                    }
                }
            }
            return false;
        }
        #endregion

        #endregion

        #region Private methods

        /// <summary>
        /// Carrega na sessão a entidade UsuarioLogado, atráves do FormsIdentity.
        /// </summary>
        private static void LoadUsuarioLogado()
        {
            try
            {
                if (HttpContext.Current.User.Identity.IsAuthenticated)
                {
                    UsuarioLogado usuLogado = new UsuarioLogado();

                    //var identity = HttpContext.Current.User.Identity as FormsIdentity;
                    //if (identity != null)
                    //{
                    //	// Recupera Ticket de autenticação gravado em Cookie
                    //	FormsIdentity id = identity;
                    //	FormsAuthenticationTicket ticket = id.Ticket;

                    //	// Carrega usuário pelo Ticket da authenticação
                    //	usuLogado.Usuario = new SYS_Usuario
                    //	{
                    //		ent_id = new Guid(UtilBO.GetNameFormsAuthentication(ticket.Name, UtilBO.TypeName.Entidade))
                    //		,
                    //		usu_login = UtilBO.GetNameFormsAuthentication(ticket.Name, UtilBO.TypeName.Login)
                    //	};

                    var identity = HttpContext.Current.User.Identity;
                    var entityId = identity.GetEntityId();
                    var usuLogin = identity.GetUsuLogin();
                    if (identity != null && entityId != null && usuLogin != null)
                    {
                        usuLogado.Usuario = new SYS_Usuario
                        {
                            ent_id = new Guid(entityId)
                            ,
                            usu_login = usuLogin
                        };

                        SYS_UsuarioBO.GetSelectBy_ent_id_usu_login(usuLogado.Usuario);

                        // Carrega grupo na session através do ticket de autenticação
                        //string gru_id = UtilBO.GetNameFormsAuthentication(ticket.Name, UtilBO.TypeName.Grupo);
                        var gru_id = identity.GetGrupoId();

                        if (!string.IsNullOrEmpty(gru_id))
						{
							usuLogado.Grupo = SYS_GrupoBO.GetEntity(new SYS_Grupo { gru_id = new Guid(gru_id) });
						}                        

                        usuLogado.Nome = PES_PessoaBO.GetEntity(new PES_Pessoa { pes_id = usuLogado.Usuario.pes_id }).pes_nome ?? usuLogado.Usuario.usu_login;
                        usuLogado.Sistemas = SYS_SistemaBO.GetSelectBy_usu_id(usuLogado.Usuario.usu_id);

                        if (usuLogado.Usuario.pes_id != Guid.Empty)
                        {
                            ACA_AlunoBusiness alunoBO = new ACA_AlunoBusiness();
                            ACA_Aluno aluno = alunoBO.GetStudentByPesId(usuLogado.Usuario.pes_id);

                            if (aluno != null && aluno.alu_id > 0)
                            {
                                usuLogado.alu_id = aluno.alu_id;
                            }
                        }

                        UsuarioLogado = usuLogado;
                    }
                }
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
            }
        }

        #endregion
    }
}
