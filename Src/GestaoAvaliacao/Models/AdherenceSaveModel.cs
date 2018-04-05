using System.Collections.Generic;

namespace GestaoAvaliacao.Models
{
    public class AdherenceSaveModel
	{
		public long test_id { get; set; }
		public List<SelectedSchools> SelectedSchools { get; set; }
		public List<int> SelectedSections { get; set; }
		public List<int> RemoverSchools { get; set; }
		public List<int> RemovedSections { get; set; }
	}

	public class SelectedSchools
	{
		public int esc_id { get; set; }
		public Entities.Enumerator.EnumAdherenceSelection TypeSelection { get; set; }
	}
}