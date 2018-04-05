using AvaliaMais.FolhaRespostas.Application.Adapters;
using AvaliaMais.FolhaRespostas.Domain.ProcessamentoInicial;
using AvaliaMais.FolhaRespostas.Domain.ProcessamentoInicial.Interfaces;
using AvaliaMais.FolhaRespostas.Domain.ProcessamentoProva;
using AvaliaMais.FolhaRespostas.Domain.ProcessamentoProva.Interfaces;
using GestaoAvaliacao.LogFacade;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AvaliaMais.FolhaRespostas.Application.TemplateMethod
{
    public abstract class AbstractProcessamento
	{
		public IProcessamentoInicialService processamentoInicialService;
		public IProcessamentoProvaService processamentoProvaService;

		private IEnumerable<Processamento> procInicial;
		private int _provaId;
		protected abstract IEnumerable<Processamento> CarregarProcessamento(int provaId);

		public bool Adicionar(int provaId)
		{
			try
			{
				_provaId = provaId;
				procInicial = CarregarProcessamento(provaId);
				SalvarMongoDb();
				return true;
			}
			catch (Exception e)
			{
				LogFacade.SaveError(e);
				return false;
			}
		}

		private void SalvarMongoDb()
		{
			var processamentoProva = ObterProcessamentoProva();
			if (processamentoProva.Count > 0)
			{
				processamentoProvaService.AdicionarProcessamentoProva(processamentoProva.FirstOrDefault());
			}
		}

		private ICollection<ProcessamentoProva> ObterProcessamentoProva()
		{
			var provasIniciais = procInicial.GroupBy(p => p.ProvaId)
							.Select(grp => grp.First())
							.ToList();

			var provas = new List<ProcessamentoProva>();
			foreach (var provaInicial in provasIniciais)
			{
				var prova = new ProcessamentoProva();
				prova.ProvaId = provaInicial.ProvaId;
				prova.ProvaNome = provaInicial.ProvaNome;
				prova.dres = ObterDresPorProva();

				provas.Add(prova);
			}
			return provas;
		}

		private ICollection<DRE> ObterDresPorProva()
		{
			var dresIniciais = procInicial.Where(p => p.ProvaId == _provaId)
				.Select(p => new { DreId = p.DreId, DreNome = p.DreNome, QtdeAlunosDre = p.QtdeAlunosDre })
				.Distinct()
				.ToList();

			var dres = new List<DRE>();
			foreach (var dreInicial in dresIniciais)
			{
				var escolas = ObterEscolasDaDre(dreInicial.DreId);
				var dre = ProcessamentoProvaAdapter.EscolaToDre(escolas);
				dre.AlunoStatus.Aderidos = dreInicial.QtdeAlunosDre;
				dre.ProvaId = _provaId;
				dre.DreId = dreInicial.DreId;
				dre.Nome = dreInicial.DreNome;
				dres.Add(dre);
			}
			return dres;
		}

		private ICollection<Escola> ObterEscolasDaDre(Guid dreId)
		{
			var escolasIniciais = procInicial.Where(p => p.DreId == dreId)
							.Select(p => new { EscolaId = p.EscolaId,
								EscolaNome = p.EscolaNome,
								EscolaUad = p.EscolaUad,
								QtdeAlunosEscola = p.QtdeAlunosEscola })
							.Distinct()
							.ToList();

			var escolas = new List<Escola>();
			foreach (var escolaInicial in escolasIniciais)
			{
				var turmas = ObterTurmasDaEscola(escolaInicial.EscolaId);
				var escola = ProcessamentoProvaAdapter.TurmaToEscola(turmas);
				escola.AlunoStatus.Aderidos = escolaInicial.QtdeAlunosEscola;
				escola.EscolaUad = escolaInicial.EscolaUad;
				escola.ProvaId = _provaId;
				escola.DreId = dreId;
				escola.EscolaId = escolaInicial.EscolaId;
				escola.Nome = escolaInicial.EscolaNome;

				escolas.Add(escola);
			}
			return escolas;
		}

		private ICollection<Turma> ObterTurmasDaEscola(int escolaId)
		{
			var turmasProvas = procInicial.Where(p => p.EscolaId == escolaId)
				.Select(p => new { TurmaId = p.TurmaId,
					ProvaId = p.ProvaId, 
					TurmaNome = p.TurmaNome,
					QtdeAlunosTurma = p.QtdeAlunosTurma })
				.Distinct()
				.ToList();

			var turmas = new List<Turma>();
            string turmaIds = turmasProvas.Select(p => Convert.ToString(p.TurmaId)).Aggregate((i, j) => i + "," + j);

            if (!string.IsNullOrEmpty(turmaIds))
            {
                var alunosEscola = processamentoInicialService.ObterAlunosPorTurmaProva(turmaIds, turmasProvas.FirstOrDefault().ProvaId).ToList();

                foreach (var turmaProva in turmasProvas)
                {
                    var alunosTurma = alunosEscola.FindAll(p => p.TurmaId == turmaProva.TurmaId && p.ProvaId == turmaProva.ProvaId).ToList();
                    var turma = ProcessamentoProvaAdapter.AlunosToTurma(alunosTurma);
                    turma.AlunoStatus.Aderidos = turmaProva.QtdeAlunosTurma;
                    turma.ProvaId = _provaId;
                    turma.EscolaId = escolaId;
                    turma.TurmaId = turmaProva.TurmaId;
                    turma.Nome = turmaProva.TurmaNome;
                    turmas.Add(turma);
                }
            }
			return turmas;
		}
	}
}
