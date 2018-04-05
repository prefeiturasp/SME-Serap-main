using System;
using System.Collections.Generic;

namespace AvaliaMais.FolhaRespostas.Domain.ProcessamentoProva
{
    public class ProcessamentoProva
	{
		public Guid _id { get; set; }
		public int ProvaId { get; set; }
		public string ProvaNome { get; set; }
		public ICollection<DRE> dres { get; set; }

		public ProcessamentoProva()
		{
			dres = new List<DRE>();
		}
	}
}
