using GestaoAvaliacao.App_Start;
using GestaoAvaliacao.WebProject.Facade;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace GestaoAvaliacao.Controllers
{
    public class ProvaSPController : Controller
    {
        [AllowAnonymous]
        // GET: ProvaSP
        public ActionResult Index()
        {
            if (Request.QueryString["App"] != null)
            {
                string usu_login="";
                string gru_nome="";
                if (Request.QueryString["usu_login"]!=null)
                {
                    usu_login = Request.QueryString["usu_login"];
                }
                else
                {
                    usu_login = SessionFacade.UsuarioLogado.Usuario.usu_login;
                    gru_nome = SessionFacade.UsuarioLogado.Grupo.gru_nome.ToLower();
                }
                var usuario = ProvaSP.Data.DataUsuario.RetornarUsuario(usu_login, "");
                var jsonUsuario = Newtonsoft.Json.JsonConvert.SerializeObject(usuario);
                //var jsonUsuario = Newtonsoft.Json.JsonConvert.SerializeObject(SessionFacade.UsuarioLogado.Usuario);
                var sbRetorno = new StringBuilder();
                sbRetorno.Append("var grupoSerap='");
                sbRetorno.Append(gru_nome);
                sbRetorno.AppendLine("';");
                sbRetorno.Append("var jsonUsuario=JSON.parse('");
                sbRetorno.Append(jsonUsuario);
                sbRetorno.AppendLine("');");
                ViewBag.JsSerap = sbRetorno.ToString();
                return View("IndexApp");
            }
            else
                return View("IndexAppContainer");
        }
    }
}