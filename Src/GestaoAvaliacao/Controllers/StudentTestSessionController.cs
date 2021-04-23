using GestaoAvaliacao.App_Start;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.Util;
using GestaoAvaliacao.WebProject.Facade;
using GestaoEscolar.Entities;
using GestaoEscolar.IBusiness;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace GestaoAvaliacao.Controllers
{
    [Authorize]
    [AuthorizeModule]
    public class StudentTestSessionController : Controller
    {
        private readonly IStudentTestAccoplishmentBusiness studentTestAccoplishmentBusiness;

        public StudentTestSessionController(IStudentTestAccoplishmentBusiness studentTestAccoplishmentBusiness)
        {
            this.studentTestAccoplishmentBusiness = studentTestAccoplishmentBusiness;
        }

        public ActionResult Index(long test_id, long team_id, long esc_id) => View();

        [HttpGet]
        public JsonResult GetStudentsSession(long test_id, long team_id, long esc_id)
        {
            var tempoDaSessao1 = new { HoraInicial = "13/12/2021 10:00", HoraFinal = "13/12/2021 10:05", Tempo = "00:05" };
            var tempoDaSessao2 = new { HoraInicial = "13/12/2021 11:00", HoraFinal = "13/12/2021 11:05", Tempo = "00:05" };
            var tempoDaSessao3 = new { HoraInicial = "13/12/2021 12:00", HoraFinal = "13/12/2021 12:05", Tempo = "00:05" };
            var sessoes = new[] { tempoDaSessao1, tempoDaSessao2, tempoDaSessao3 }.ToList();
            var aluno1 = new { 
                NumeroDaChamada = "01",
                NomeDoAluno = "Jdulley Castro de Freitas",
                TempoTotalDaSessao = "10:00",
                TempoDasSessoes = sessoes
            };
            var aluno2 = new
            {
                NumeroDaChamada = "02",
                NomeDoAluno = "Jdulley Castro de Freitas",
                TempoTotalDaSessao = "10:00",
                TempoDasSessoes = sessoes
            };
            var aluno3 = new
            {
                NumeroDaChamada = "03",
                NomeDoAluno = "Jdulley Castro de Freitas",
                TempoTotalDaSessao = "10:00",
                TempoDasSessoes = sessoes
            };
            var retorno = new[] { aluno1, aluno2, aluno3 }.ToList();
            return Json(new { success = true, dados = retorno }, JsonRequestBehavior.AllowGet);
        }
    }
}