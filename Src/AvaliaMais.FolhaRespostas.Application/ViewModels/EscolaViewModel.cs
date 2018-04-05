using AvaliaMais.FolhaRespostas.Domain.ProcessamentoProva;
using System.Collections.Generic;

namespace AvaliaMais.FolhaRespostas.Application.ViewModels
{
    public class EscolaViewModel
	{
		public Quantidade QuantidadeTotal { get; set; }
		public IEnumerable<Escola> lista { get; set; }
		public bool Success { get; set; }
	}
}
