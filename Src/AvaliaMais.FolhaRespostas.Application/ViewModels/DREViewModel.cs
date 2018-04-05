using AvaliaMais.FolhaRespostas.Domain.ProcessamentoProva;
using System.Collections.Generic;

namespace AvaliaMais.FolhaRespostas.Application.ViewModels
{
    public class DREViewModel
	{
		public Quantidade QuantidadeTotal { get; set; }
		public IEnumerable<DRE> lista { get; set; }
		public bool Success { get; set; }
	}
}
