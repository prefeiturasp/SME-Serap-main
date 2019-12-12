using ProvaSP.Data;
using ProvaSP.Web.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProvaSP.Web.Controllers
{
    public class RelatorioAcompanhamentoNivelSMEController : Controller
    {
        // GET: RelatorioAcompanhamentoNivelSME
        public ActionResult Index(string usu_id)
        {
            var usuario = DataUsuario.RetornarUsuario(usu_id);

            if (usuario.AcessoNivelSME)
            {
                ViewBag.Usuario = usuario;
                var model = new RelatorioAcompanhamentoEscola();
                var indicadoresSME = Data.DataAcompanhamentoAplicacao.RecuperarAcompanhamentoEscolaNivelSME(Data.Funcionalidades.Prova.Edicao);
                model.IndicadoresAgrupadosChave = Data.DataAcompanhamentoAplicacao.MontarGridQuantidadeRespondentes(indicadoresSME);
                return View(model);
            }
            else
                return RedirectToAction("Index", "RelatorioAcompanhamento");
        }
    }
}