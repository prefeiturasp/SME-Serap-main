using System;
using System.Collections.Generic;

namespace AvaliaMais.FolhaRespostas.Domain.ProcessamentoProva
{
    public class Escola
	{
		public Guid _id { get; set; }
		public int EscolaId { get; set; }
		public Guid EscolaUad { get; set; }
		public int ProvaId { get; set; }
		public string Nome { get; set; }
		public AlunoStatus AlunoStatus { get; set; }
		public ProcessamentoStatus ProcessamentoStatus { get; set; }
		public Guid DreId { get; set; }
		public ICollection<Turma> Turmas { get; set; }
		public bool Ativo { get; set; }        

        public Escola()
		{
			AlunoStatus = new AlunoStatus();
			ProcessamentoStatus = new ProcessamentoStatus();
			Turmas = new List<Turma>();
			Ativo = true;
		}
	}
}
