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
    public class RelatorioAcompanhamentoNivelEscolaController : Controller
    {
        // GET: RelatorioAcompanhamentoNivelEscola
        public ActionResult Index(string usu_id, string esc_codigo)
        {
            var usuario = DataUsuario.RetornarUsuario(usu_id);

            if (usuario.AcessoNivelSME || usuario.Supervisor || usuario.Diretor || usuario.Coordenador)
            {
                ViewBag.Usuario = usuario;
                var escola = DataEscola.RecuperarEscola(esc_codigo);
                ViewBag.Escola = escola;
                ViewBag.Usuario = usuario;
                var model = new RelatorioAcompanhamentoEscola();
                model.IndicadoresEscola = Data.DataAcompanhamentoAplicacao.RecuperarAcompanhamentoEscolaNivelEscola(Data.Funcionalidades.Prova.Edicao, esc_codigo);
                model.IndicadoresTurma = Data.DataAcompanhamentoAplicacao.RecuperarAcompanhamentoTurmaNivelEscola(Data.Funcionalidades.Prova.Edicao, esc_codigo);
                model.IndicadoresPessoa = Data.DataAcompanhamentoAplicacao.RecuperarAcompanhamentoPessoaNivelEscola(Data.Funcionalidades.Prova.Edicao, esc_codigo);
                return View(model);
            }
            else
                return RedirectToAction("Index", "RelatorioAcompanhamento");
        }
    }
}