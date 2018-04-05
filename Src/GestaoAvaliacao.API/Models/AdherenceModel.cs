using GestaoAvaliacao.Entities.Enumerator;
using System.Collections.Generic;

namespace GestaoAvaliacao.API.Models
{
    public class AdherenceModel
	{
		public long idEntity { get; set; }
		public EnumAdherenceEntity typeEntity { get; set; }
		public EnumAdherenceSelection typeSelection { get; set; }
		public int ttn_id { get; set; }
		public int year { get; set; }
        public long parentId { get; set; }
    }

	public class AdherenceListModel
	{
		public List<long> entityList { get; set; }
		public EnumAdherenceSelection typeSelection { get; set; }
		public int ttn_id { get; set; }
		public int year { get; set; }
	}
}