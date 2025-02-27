using ProvaSP.Data;
using ProvaSP.Model.Entidades;
using ProvaSP.Web.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProvaSP.Web.Controllers
{
    public class RelatorioAcompanhamentoNivelDREController : Controller
    {
        // GET: RelatorioAcompanhamentoNivelDRE
        public ActionResult Index(string usu_id, string uad_codigo)
        {
            var usuario = DataUsuario.RetornarUsuario(usu_id);

            if (usuario.AcessoNivelSME || usuario.AcessoNivelDRE || usuario.Supervisor)
            {
                ViewBag.Usuario = usuario;
                ViewBag.DRE = uad_codigo;
                var model = new RelatorioAcompanhamentoEscola();
                var indicadoresEscola = Data.DataAcompanhamentoAplicacao.RecuperarAcompanhamentoEscolaNivelDRE(Data.Funcionalidades.Prova.Edicao, uad_codigo);
                model.IndicadoresAgrupadosChave = Data.DataAcompanhamentoAplicacao.MontarGridQuantidadeRespondentes(indicadoresEscola);
                return View(model);
            }
            else
                return RedirectToAction("Index", "RelatorioAcompanhamento");
        }
    }
}