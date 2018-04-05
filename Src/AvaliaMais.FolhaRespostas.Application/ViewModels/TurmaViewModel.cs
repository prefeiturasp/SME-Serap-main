using AvaliaMais.FolhaRespostas.Domain.ProcessamentoProva;
using System.Collections.Generic;

namespace AvaliaMais.FolhaRespostas.Application.ViewModels
{
    public class TurmaViewModel
	{
		public Quantidade QuantidadeTotal { get; set; }
		public IEnumerable<Turma> lista { get; set; }
		public bool Success { get; set; }
	}
}
