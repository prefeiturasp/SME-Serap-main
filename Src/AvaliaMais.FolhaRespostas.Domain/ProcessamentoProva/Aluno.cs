using System;

namespace AvaliaMais.FolhaRespostas.Domain.ProcessamentoProva
{
    public class Aluno
	{
		public Guid _id { get; set; }
		public int AlunoId { get; set; }
		public string Nome { get; set; }
		public int Numero { get; set; }
		public DateTime? DataMatricula { get; set; }
		public DateTime? DataSaida { get; set; }
		public Situacao? Situacao { get; set; }
		public int TurmaId { get; set; }
		public bool Ativo { get; set; }
		public int ProvaId { get; set; }
		public bool Ausente { get; set; }

		public Aluno()
		{
			Ativo = true;
		}
	}

	public enum Situacao
	{
		Pendente = 1,
		Sucesso = 4,
		Erro = 5,
		Conferir = 6
	}
}
