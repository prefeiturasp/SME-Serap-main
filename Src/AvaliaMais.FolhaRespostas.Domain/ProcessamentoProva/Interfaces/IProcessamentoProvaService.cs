using GestaoAvaliacao.Util;
using System;
using System.Collections.Generic;

namespace AvaliaMais.FolhaRespostas.Domain.ProcessamentoProva.Interfaces
{
    public interface IProcessamentoProvaService
	{
		void AdicionarProcessamentoProva(ProcessamentoProva prova);

		bool DeletarInativos();
		bool InativarDocumentos(int provaId);

		IEnumerable<DRE> ObterDres(ref Pager pagina, int provaId);
		IEnumerable<DRE> ObterDresGestor(ref Pager pagina, int provaId, Guid[] dresIds);
		IEnumerable<Escola> ObterEscola(ref Pager pagina, int provaId, Guid DreId);
		IEnumerable<Escola> ObterEscolaDiretor(ref Pager pagina, int provaId, Guid[] uadsIds);
		IEnumerable<Turma> ObterTurmas(ref Pager pagina, int provaId, int escolaId);
		IEnumerable<Turma> ObterTurmasProfessor(ref Pager pagina, int provaId, int[] turmas);
		IEnumerable<Aluno> ObterAlunos(ref Pager pagina, int turmaId, int provaId);

        IEnumerable<DRE> ObterDresSemPaginacao(int provaId);
        IEnumerable<DRE> ObterDresGestorSemPaginacao(int provaId, Guid[] dresIds);
        IEnumerable<Escola> ObterEscolaSemPaginacao(int provaId, Guid DreId);
        IEnumerable<Escola> ObterEscolaDiretorSemPaginacao(int provaId, Guid[] uadsIds);

        Quantidade QuantidadeDre(int provaId);
		Quantidade QuantidadeDreGestor(int provaId, Guid[] dresIds);
		Quantidade QuantidadeEscola(int provaId, Guid DreId);
		Quantidade QuantidadeEscolaDiretor(int provaId, Guid[] uadsIds);
		Quantidade QuantidadeTurma(int provaId, int escolaId);
		Quantidade QuantidadeTurmaProfessor(int provaId, int[] turmas);

		IEnumerable<DRE> ObterDres(int provaId);
		IEnumerable<DRE> ObterDresGestor(int provaId, Guid[] dresIds);
		IEnumerable<Escola> ObterEscolas(int provaId, Guid DreId);
		IEnumerable<Escola> ObterEscolaDiretor(int provaId, Guid[] uadsIds);
		IEnumerable<Turma> ObterTurmas(int provaId, int escolaId);
		IEnumerable<Turma> ObterTurmasProfessor(int provaId, int[] turmas);
		IEnumerable<Aluno> ObterAlunos(int turmaId, int provaId);
	}
}
