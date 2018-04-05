using System;

namespace AvaliaMais.FolhaRespostas.Domain.ProcessamentoInicial
{
    public class Processamento
	{
		public int ProvaId { get; set; }
		public string ProvaNome { get; set; }
		public Guid DreId { get; set; }
		public int EscolaId { get; set; }
		public Guid EscolaUad { get; set; }
		public int TurmaId { get; set; }
		public int Situacao { get; set; }
		public int QtdeTurma { get; set; }
		public int QtdeEscola { get; set; }
		public int QtdeDre { get; set; }
		public string DreNome { get; set; }
		public string EscolaNome { get; set; }
		public string TurmaNome { get; set; }
		public int QtdeAlunosTurma { get; set; }
		public int QtdeAlunosEscola { get; set; }
		public int QtdeAlunosDre { get; set; }
	}
}
