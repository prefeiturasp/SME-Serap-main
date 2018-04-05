using GestaoAvaliacao.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestaoAvaliacao.Entities
{
    public class CorrectionStudentGrid
	{
		public long alu_id { get; set; }
		public string alu_nome { get; set; }
		public long? AbsenceReason_id { get; set; }
		public int mtu_numeroChamada { get; set; }
		public string FileName { get; set; }
		public string FileOriginalName { get; set; }
		public long FileId { get; set; }
		public string FilePath { get; set; }
		public int QtdeItem { get; set; }
		public int Correcteds { get; set; }
		public List<StudentCorrectionAnswerGrid> Items { get; set; }
		public bool Automatic { get; set; }
        public Guid dre_id { get; set; }
        public int esc_id { get; set; }
        public int cur_id { get; set; }
        public int crr_id { get; set; }
        public int crp_id { get; set; }
        public bool blocked { get; set; }

        [NotMapped]
        public StatusProvaEletronica status { get; set; }
    }
}
