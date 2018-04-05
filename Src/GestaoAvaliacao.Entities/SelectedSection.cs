using GestaoAvaliacao.Util;
using System;

namespace GestaoAvaliacao.Entities
{
    public class SelectedSection
	{
		public long tur_id { get; set; }
		public string tur_codigo { get; set; }
        public string esc_nome { get; set; }
        public string uad_nome { get; set; }
        public int esc_id { get; set; }
        public Guid? dre_id { get; set; }
        public string ttn_nome { get; set; }
		public EnumStatusCorrection StatusCorrection { get; set; }
        public string FileName { get; set; }
        public string FileOriginalName { get; set; }
        public long FileId { get; set; }
        public string FilePath { get; set; }
	}
}
