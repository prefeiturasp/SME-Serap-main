using ProvaSP.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProvaSP.Web.Controllers
{
    public class RelatorioAcompanhamentoNivelSME_PorDREController : Controller
    {
        // GET: RelatorioAcompanhamentoNivelSME_PorDRE
        public ActionResult Index(string usu_id)
        {
            var usuario = DataUsuario.RetornarUsuario(usu_id);

            if (usuario.AcessoNivelSME)
            {
                ViewBag.Usuario = usuario;
                var indicadores = Data.DataAcompanhamentoAplicacao.RecuperarAcompanhamentoEscolaNivelSME_PorDRE("2018");
                return View(indicadores);
            }
            else
                return RedirectToAction("Index", "RelatorioAcompanhamento");
        }
    }
}