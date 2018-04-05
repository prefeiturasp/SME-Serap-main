using AvaliaMais.FolhaRespostas.Domain.ProcessamentoProva.Interfaces;
using GestaoAvaliacao.Util;
using System;
using System.Collections.Generic;

namespace AvaliaMais.FolhaRespostas.Domain.ProcessamentoProva.Services
{
    public class ProcessamentoProvaService : IProcessamentoProvaService
	{
		private readonly IProcessamentoProvaRepository _processamentoProvaRepository;

		public ProcessamentoProvaService(IProcessamentoProvaRepository processamentoProvaRepository)
		{
			_processamentoProvaRepository = processamentoProvaRepository;
		}

		public void AdicionarProcessamentoProva(ProcessamentoProva prova)
		{
			_processamentoProvaRepository.AdicionarProcessamentoProva(prova);
		}

		public bool DeletarInativos()
		{
			return _processamentoProvaRepository.DeletarInativos();
		}

		public bool InativarDocumentos(int provaId)
		{
			return _processamentoProvaRepository.InativarDocumentos(provaId);
		}

		public IEnumerable<DRE> ObterDres(ref Pager pagina, int provaId)
		{
			return _processamentoProvaRepository.ObterDres(ref pagina, provaId);
		}

		public IEnumerable<DRE> ObterDresGestor(ref Pager pagina, int provaId, Guid[] dresIds)
		{
			return _processamentoProvaRepository.ObterDresGestor(ref pagina, provaId, dresIds);
		}

		public IEnumerable<Escola> ObterEscola(ref Pager pagina, int provaId, Guid DreId)
		{
			return _processamentoProvaRepository.ObterEscolas(ref pagina, provaId, DreId);
		}

		public IEnumerable<Escola> ObterEscolaDiretor(ref Pager pagina, int provaId, Guid[] uadsIds)
		{
			return _processamentoProvaRepository.ObterEscolasDiretor(ref pagina, provaId, uadsIds);
		}

		public IEnumerable<Turma> ObterTurmas(ref Pager pagina, int provaId, int escolaId)
		{
			return _processamentoProvaRepository.ObterTurmas(ref pagina, provaId, escolaId);
		}

		public IEnumerable<Turma> ObterTurmasProfessor(ref Pager pagina, int provaId, int[] turmas)
		{
			return _processamentoProvaRepository.ObterTurmasProfessor(ref pagina, provaId, turmas);
		}

		public IEnumerable<Aluno> ObterAlunos(ref Pager pagina, int turmaId, int provaId)
		{
			return _processamentoProvaRepository.ObterAlunos(ref pagina, turmaId, provaId);
		}

        public IEnumerable<DRE> ObterDresSemPaginacao(int provaId)
        {
            return _processamentoProvaRepository.ObterDresSemPaginacao(provaId);
        }

        public IEnumerable<DRE> ObterDresGestorSemPaginacao(int provaId, Guid[] dresIds)
        {
            return _processamentoProvaRepository.ObterDresGestorSemPaginacao(provaId, dresIds);
        }

        public IEnumerable<Escola> ObterEscolaSemPaginacao(int provaId, Guid DreId)
        {
            return _processamentoProvaRepository.ObterEscolasSemPaginacao(provaId, DreId);
        }

        public IEnumerable<Escola> ObterEscolaDiretorSemPaginacao(int provaId, Guid[] uadsIds)
        {
            return _processamentoProvaRepository.ObterEscolasDiretorSemPaginacao(provaId, uadsIds);
        }

        public Quantidade QuantidadeDre(int provaId)
		{
			return _processamentoProvaRepository.QuantidadeDre(provaId);
		}

		public Quantidade QuantidadeDreGestor(int provaId, Guid[] dresIds)
		{
			return _processamentoProvaRepository.QuantidadeDreGestor(provaId, dresIds);
		}

		public Quantidade QuantidadeEscola(int provaId, Guid DreId)
		{
			return _processamentoProvaRepository.QuantidadeEscola(provaId,DreId);
		}

		public Quantidade QuantidadeEscolaDiretor(int provaId, Guid[] uadsIds)
		{
			return _processamentoProvaRepository.QuantidadeEscolaDiretor(provaId, uadsIds);
		}

		public Quantidade QuantidadeTurma(int provaId, int escolaId)
		{
			return _processamentoProvaRepository.QuantidadeTurma(provaId, escolaId);
		}

		public Quantidade QuantidadeTurmaProfessor(int provaId, int[] turmas)
		{
			return _processamentoProvaRepository.QuantidadeTurmaProfessor(provaId, turmas);
		}

		public IEnumerable<DRE> ObterDres(int provaId)
		{
			return _processamentoProvaRepository.ObterDres(provaId);
		}

		public IEnumerable<DRE> ObterDresGestor(int provaId, Guid[] dresIds)
		{
			return _processamentoProvaRepository.ObterDresGestor(provaId, dresIds);
		}

		public IEnumerable<Escola> ObterEscolas(int provaId, Guid DreId)
		{
			return _processamentoProvaRepository.ObterEscolas(provaId, DreId);
		}

		public IEnumerable<Escola> ObterEscolaDiretor(int provaId,Guid[] uadsIds)
		{
			return _processamentoProvaRepository.ObterEscolaDiretor(provaId, uadsIds);
		}

		public IEnumerable<Turma> ObterTurmas(int provaId, int escolaId)
		{
			return _processamentoProvaRepository.ObterTurmas(provaId, escolaId);
		}

		public IEnumerable<Turma> ObterTurmasProfessor(int provaId, int[] turmas)
		{
			return _processamentoProvaRepository.ObterTurmasProfessor(provaId, turmas);
		}

		public IEnumerable<Aluno> ObterAlunos(int turmaId, int provaId)
		{
			return _processamentoProvaRepository.ObterAlunos(turmaId, provaId);
		}
	}
}
