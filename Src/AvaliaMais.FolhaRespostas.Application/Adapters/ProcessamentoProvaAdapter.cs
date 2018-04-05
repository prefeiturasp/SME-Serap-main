using AvaliaMais.FolhaRespostas.Domain.ProcessamentoProva;
using System.Collections.Generic;
using System.Linq;

namespace AvaliaMais.FolhaRespostas.Application.Adapters
{
    public static class ProcessamentoProvaAdapter
	{
		public static Turma AlunosToTurma(ICollection<Aluno> alunos)
		{
			var turma = new Turma();

			turma.Alunos = alunos;
			turma.AlunoStatus.Identificados = alunos.Where(a => a.Situacao != null && a.Situacao != Situacao.Pendente).Count();

			turma.ProcessamentoStatus.Sucesso = alunos.Where(a => a.Situacao != null && a.Situacao.Value == Situacao.Sucesso).Count();
			turma.ProcessamentoStatus.Pendente = alunos.Where(a => a.Situacao != null && a.Situacao.Value == Situacao.Pendente).Count();
			turma.ProcessamentoStatus.Erro = alunos.Where(a => a.Situacao != null && a.Situacao.Value == Situacao.Erro).Count();
			turma.ProcessamentoStatus.Conferir = alunos.Where(a => a.Situacao != null && a.Situacao.Value == Situacao.Conferir).Count();
			turma.ProcessamentoStatus.Ausente = alunos.Where(a => a.Ausente).Count();

			return turma;
		}

		public static Escola TurmaToEscola(ICollection<Turma> turmas)
		{
			var escola = new Escola();

			escola.Turmas = turmas;
			escola.AlunoStatus.Identificados = turmas.Sum(t => t.AlunoStatus.Identificados);
			escola.ProcessamentoStatus.Sucesso = turmas.Sum(t => t.ProcessamentoStatus.Sucesso);
			escola.ProcessamentoStatus.Ausente = turmas.Sum(t => t.ProcessamentoStatus.Ausente);
			escola.ProcessamentoStatus.Erro = turmas.Sum(t => t.ProcessamentoStatus.Erro);
			escola.ProcessamentoStatus.Pendente = turmas.Sum(t => t.ProcessamentoStatus.Pendente);
			escola.ProcessamentoStatus.Conferir = turmas.Sum(t => t.ProcessamentoStatus.Conferir);

			return escola;
		}

		public static DRE EscolaToDre(ICollection<Escola> escolas)
		{
			var dre = new DRE();

			dre.Escolas = escolas;
			dre.AlunoStatus.Identificados = escolas.Sum(t => t.AlunoStatus.Identificados);
			dre.ProcessamentoStatus.Sucesso = escolas.Sum(t => t.ProcessamentoStatus.Sucesso);
			dre.ProcessamentoStatus.Ausente = escolas.Sum(t => t.ProcessamentoStatus.Ausente);
			dre.ProcessamentoStatus.Erro = escolas.Sum(t => t.ProcessamentoStatus.Erro);
			dre.ProcessamentoStatus.Pendente = escolas.Sum(t => t.ProcessamentoStatus.Pendente);
			dre.ProcessamentoStatus.Conferir = escolas.Sum(t => t.ProcessamentoStatus.Conferir);

			return dre;
		}
	}
}
