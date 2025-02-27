using Castle.Windsor;
using GestaoAvaliacao.Entities;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.MappingDependence;
using GestaoAvaliacao.Util;
using MSTech.CoreSSO.BLL;
using MSTech.CoreSSO.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Linq;

namespace GestaoAvaliacao.WebProject.Facade
{
    public static class ApplicationFacade
	{
		private static IWindsorContainer container;
		private const string VERSAO = "VERSAO";
		private const string GRUPOS_PERMISSOES = "GRUPOS_PERMISSOES";
		private const string MODULOS_GRUPOS_PERMISSOES = "MODULOS_GRUPOS_PERMISSOES";
		private const string PARAMETERS = "PARAMETERS";
		private const string PARAMETERKEYS = "PARAMETERKEYS";
        private const string SME_ENT_ID = "6CF424DC-8EC3-E011-9B36-00155D033206";

        #region Constructor

        static ApplicationFacade()
		{
			// Armazena a URL do SSO definido nos parâmetros do CoreSSO.
			UrlCoreSso = SYS_ParametroBO.ParametroValor(SYS_ParametroBO.eChave.URL_ADMINISTRATIVO);

			var sistema = SYS_SistemaBO.GetEntity(new SYS_Sistema { sis_id = Constants.IdSistema });
			
			// Armazena o Título do sistema, disponível na tabela SYS_Sitema do CoreSSO.
			TituloSistema = sistema.sis_nome;

			// Armazena a descrição do sistema, disponível na tabela SYS_Sitema do CoreSSO.
			DescricaoSistema = sistema.sis_descricao;

			// Armazena a url do logo do cabecalho, disponível na tabela SYS_Sitema do CoreSSO.
			UrlLogoCabecalho = UtilBO.UrlImagemGestao(UrlCoreSso, SYS_SistemaBO.GetEntity(new SYS_Sistema { sis_id = Constants.IdSistema }).sis_urlLogoCabecalho);

			container = new WindsorContainer()
				.Install(new BusinessInstaller())
				.Install(new RepositoriesInstaller());
		}

		#endregion

		#region Public Methods

		///<sumary>
		///Permissões de grupos existentes
		/// </sumary>
		private static Dictionary<KeyValuePair<Guid, int>, List<Entities.Menu>> GruposPermissoes
		{
			get
			{
				if (HttpContext.Current.Application[GRUPOS_PERMISSOES] == null)
					HttpContext.Current.Application[GRUPOS_PERMISSOES] = new Dictionary<KeyValuePair<Guid, int>, List<Entities.Menu>>();

				return (Dictionary<KeyValuePair<Guid, int>, List<Entities.Menu>>)HttpContext.Current.Application[GRUPOS_PERMISSOES];
			}
			set
			{
				HttpContext.Current.Application[GRUPOS_PERMISSOES] = value;
			}
		}

		///<sumary>
		///Retorna o um dicionário de módulos por grupo
		///</sumary>
		private static Dictionary<Guid, Dictionary<string, SYS_GrupoPermissao>> ModulosGruposPermissoes
		{
			get
			{
				if (HttpContext.Current.Application[MODULOS_GRUPOS_PERMISSOES] == null)
					ModulosGruposPermissoes = new Dictionary<Guid, Dictionary<string, SYS_GrupoPermissao>>();

				return (Dictionary<Guid, Dictionary<string, SYS_GrupoPermissao>>)HttpContext.Current.Application[MODULOS_GRUPOS_PERMISSOES];
			}
			set
			{
				HttpContext.Current.Application[MODULOS_GRUPOS_PERMISSOES] = value;
			}
		}

		/// <summary>
		/// Retorna a URL do SSO disponível na tabela de parâmetros do CoreSSO.
		/// </summary>
		public static string UrlCoreSso { get; set; }

		/// <summary>
		/// Retorna a URL do SSO disponível na tabela de parâmetros do CoreSSO.
		/// </summary>
		public static string UrlLogoCabecalho { get; set; }

		/// <summary>
		/// Retorna o Título do sistema, disponível na tabela SYS_Sitema do CoreSSO.
		/// </summary>
		public static string TituloSistema { get; set; }

		/// <summary>
		/// Retorna a descrição do sistema, disponível na tabela SYS_Sitema do CoreSSO.
		/// </summary>
		public static string DescricaoSistema { get; set; }

		public static List<Parameter> Parameters
		{
			get
			{
				if (HttpContext.Current.Application[PARAMETERS] == null)
					LoadParameters();

				return (List<Parameter>)HttpContext.Current.Application[PARAMETERS];
			}
			set
			{
				HttpContext.Current.Application[PARAMETERS] = value;
			}
		}
		
        public static bool LimparGruposEPermissoes
        {
            set
            {
                if (value)
                {
                    GruposPermissoes = null;
                    ModulosGruposPermissoes = null;
                }
            }
        } 		

		/// <summary>
		/// Usado em ParametersObject.cshtml
		/// </summary>
		public static string ParameterKeys
		{
			get
			{
				if (HttpContext.Current.Application[PARAMETERKEYS] == null)
				{
					return EnumExtensions.EnumToJson<EnumParameterKey>(true);
				}

				return (string)HttpContext.Current.Application[PARAMETERKEYS];
			}
			set
			{
				HttpContext.Current.Application[PARAMETERKEYS] = value;
			}
		}

		/// <summary>
		/// Retorna a versão do site, baseado no arquivo version.xml.
		/// </summary>
		public static string Versao
		{
			get
			{
				string versao = HttpContext.Current.Application[VERSAO] != null ? HttpContext.Current.Application[VERSAO].ToString() : string.Empty;
				if (string.IsNullOrEmpty(versao))
				{
					versao = RecuperarVersao();

					try
					{
						HttpContext.Current.Application.Lock();
						HttpContext.Current.Application[VERSAO] = versao;
					}
					finally
					{
						HttpContext.Current.Application.UnLock();
					}
				}
				return versao;
			}
		}

        private static string GetPhysicalDirectoryByEntId(Guid entId)
        {
            var parameterBusiness = container.Resolve<IParameterBusiness>();
            var paramPath = parameterBusiness.GetParamByKey(EnumParameterKey.STORAGE_PATH.GetDescription(), entId);
            var physicalPath = paramPath != null ? paramPath.Value : HttpContext.Current.Request.PhysicalApplicationPath;

            return physicalPath;
        }

        private static string GetVirtualDirectoryByEntId(Guid entId)
        {
            var parameterBusiness = container.Resolve<IParameterBusiness>();
            var paramPath = parameterBusiness.GetParamByKey(EnumParameterKey.VIRTUAL_PATH.GetDescription(), entId);
            var virtualPath = paramPath != null ? paramPath.Value : (BaseURL + HttpContext.Current.Request.ApplicationPath);

            return virtualPath;
        }

		public static string PhysicalDirectory => GetPhysicalDirectoryByEntId(SessionFacade.UsuarioLogado.Usuario.ent_id);

        public static string VirtualDirectory => GetVirtualDirectoryByEntId(SessionFacade.UsuarioLogado.Usuario.ent_id);

        public static string PhysicalDirectorySme => GetPhysicalDirectoryByEntId(Guid.Parse(SME_ENT_ID));

        public static string VirtualDirectorySme => GetVirtualDirectoryByEntId(Guid.Parse(SME_ENT_ID));

        public static string ProjectVirtualDirectory => (BaseURL + HttpContext.Current.Request.ApplicationPath);

        private static string BaseURL
		{
			get
			{
				var baseURL = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
				if (baseURL.StartsWith("http://hom-")) // ambiente de homologação usa HTTPS aplicado pelo Proxy
					baseURL = baseURL.Replace("http://", "https://");
				else if (baseURL.StartsWith("http://itens-"))// ambiente de itens usa HTTPS aplicado pelo Proxy
					baseURL = baseURL.Replace("http://", "https://");
				else if(baseURL.StartsWith("http://serap.sme"))
					baseURL = baseURL.Replace("http://", "https://");

				return baseURL;
			}
		}
		
		#endregion

		#region Private Methods

		private static string RecuperarVersao()
		{
			var xmlDoc = new XmlDocument();
			
			xmlDoc.Load(HttpContext.Current.Request.PhysicalApplicationPath + "version.xml");

			var xn = xmlDoc.GetElementsByTagName("versionNumber");

			string strRet = string.Format(" {0}.{1}.{2}.{3}",
				xn[0]["Major"].GetAttribute("value"),
				xn[0]["Minor"].GetAttribute("value"),
				xn[0]["Revision"].GetAttribute("value"),
				xn[0]["Build"].GetAttribute("value"));

			return strRet;
		}

		private static void LoadParameters()
		{
			IParameterBusiness parameterBusiness = container.Resolve<IParameterBusiness>();

			var parameters = parameterBusiness.GetAll().ToList();

			Parameters = parameters;
		}

		#endregion

		#region Menu Methods

		public static List<Entities.Menu> GetMenu(Guid gru_id, int vis_id)
		{
			Dictionary<KeyValuePair<Guid, int>, List<Entities.Menu>> GrupoSection = GruposPermissoes;
			if (!GrupoSection.ContainsKey(new KeyValuePair<Guid, int>(gru_id, vis_id)))
			{
				GrupoSection.Add(new KeyValuePair<Guid, int>(gru_id, vis_id),
					MontarMenu(gru_id, vis_id));

				GruposPermissoes = GrupoSection;
			}
			return GrupoSection[new KeyValuePair<Guid, int>(gru_id, vis_id)];
		}

		private static List<Entities.Menu> MontarMenu(Guid gru_id, int vis_id)
		{
			XDocument document = XDocument.Parse(GetMenuString(gru_id, vis_id));

			List<Entities.Menu> menus = (from m in document.Descendants("menu")
										 select new Entities.Menu
										 {
											 Id = m.Attribute("id").Value,
											 Ordem = Convert.ToInt16(m.Attribute("ordem").Value),
											 Url = m.Attribute("url").Value,
											 Itens = CarregarFilhosMenu(m),
											 Icon = GetIconFromMenu(m.Attribute("id").Value)
										 }).ToList();

			return menus;
		}

		private static string GetMenuString(Guid gru_id, int vis_id)
		{
			string stringMenu = SYS_ModuloBO.CarregarMenuXML(gru_id, Constants.IdSistema, vis_id);

			stringMenu = stringMenu.Replace("<subitem", "<item");
			stringMenu = stringMenu.Replace("</subitem>", "</item>");
			stringMenu = stringMenu.Replace("<subitem2", "<item");
			stringMenu = stringMenu.Replace("<item2", "<item");

			return stringMenu;
		}

		private static List<Entities.Menu> CarregarFilhosMenu(XElement filho)
		{
			List<Entities.Menu> itens = (from f in filho.Elements()
										 select new Entities.Menu
								{
									Id = f.Attribute("id").Value,
									Ordem = Convert.ToInt16(f.Attribute("ordem").Value),
									Url = f.Attribute("url").Value,
									Itens = CarregarFilhosMenu(f)
									//,Icon = GetIconFromMenu(f.Attribute("id").Value)
								}).ToList();
			return itens;
		}

		private static string GetIconFromMenu(string p)
		{
			string retorno = null;

			EnumDescriptionMenu menu = Enum.GetValues(typeof(EnumDescriptionMenu)).Cast<EnumDescriptionMenu>().FirstOrDefault(v => v.ToString().Equals(p));
			if ((int)menu > 0)
				retorno = menu.GetDescription();
			else
				retorno = "insert_drive_file";

			return retorno;
		}

		#endregion

		#region Permission Methods

		/// <summary>
		/// Configura a permissão do usuário logado no sistema de acordo com seu grupo.
		/// </summary>
		/// <param name="virtualPath">Caminho virtual da requisição</param>
		public static SYS_GrupoPermissao SetAuthorizeModule(Guid gru_id, string virtualPath)
		{
			SYS_GrupoPermissao CurrentGrupoPermissao = null;

			Dictionary<Guid, Dictionary<string, SYS_GrupoPermissao>> modulos = ModulosGruposPermissoes;

			if (!modulos.ContainsKey(gru_id))
				modulos.Add(gru_id, new Dictionary<string, SYS_GrupoPermissao>());

			Dictionary<string, SYS_GrupoPermissao> modulosGrupo = modulos[gru_id];

			if (modulosGrupo.Keys.Contains(virtualPath))
			{
				CurrentGrupoPermissao = modulosGrupo[virtualPath];
			}
			else
			{
				SYS_GrupoPermissao grupoPermissao = SYS_GrupoBO.GetGrupoPermissaoBy_UrlNaoAbsoluta(gru_id, @"/" + virtualPath);
				modulosGrupo.Add(virtualPath, grupoPermissao);

				modulos[gru_id] = modulosGrupo;

				ModulosGruposPermissoes = modulos;
				CurrentGrupoPermissao = grupoPermissao;
			}

			return CurrentGrupoPermissao;
		}

		#endregion
	}
}
