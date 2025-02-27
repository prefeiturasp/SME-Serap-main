using GestaoAvaliacao.Entities.Enumerator;
using System;

namespace GestaoAvaliacao.Entities
{
    public class AdherenceGrid
	{
		public string uad_nome { get; set; }
		public int esc_id { get; set; }
		public string esc_nome { get; set; }
		public Nullable<EnumAdherenceSelection> TypeSelection { get; set; }
		public Guid uad_id { get; set; }
        public long alu_id { get; set; }
        public string alu_nome { get; set; }
        public bool existAdherence { get; set; }
        public Guid pes_id { get; set; }
        public string Alu_Matricula { get; set; }
    }
}
