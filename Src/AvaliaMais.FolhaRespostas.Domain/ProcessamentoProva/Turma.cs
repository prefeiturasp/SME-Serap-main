using System;
using System.Collections.Generic;

namespace AvaliaMais.FolhaRespostas.Domain.ProcessamentoProva
{
    public class Turma
	{
		public Guid _id { get; set; }
		public int TurmaId { get; set; }
		public int ProvaId { get; set; }
		public string Nome { get; set; }
		public AlunoStatus AlunoStatus { get; set; }
		public ProcessamentoStatus ProcessamentoStatus { get; set; }
		public int EscolaId { get; set; }
		public ICollection<Aluno> Alunos { get; set; }
		public bool Ativo { get; set; }

		public Turma()
		{
			AlunoStatus = new AlunoStatus();
			ProcessamentoStatus = new ProcessamentoStatus();
			Alunos = new List<Aluno>();
			Ativo = true;
		}
	}
}
