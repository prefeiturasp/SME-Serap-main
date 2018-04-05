using GestaoAvaliacao.Entities.Base;
using GestaoAvaliacao.Util;
using System;

namespace GestaoAvaliacao.Entities
{
    public class AnswerSheetBatchFiles : EntityBase
    {
        public virtual File File { get; set; }
        public long File_Id { get; set; }

        public virtual AnswerSheetBatch AnswerSheetBatch { get; set; }
        public long? AnswerSheetBatch_Id { get; set; }

        public virtual AnswerSheetBatchQueue AnswerSheetBatchQueue { get; set; }
        public long? AnswerSheetBatchQueue_Id { get; set; }

        public long? Student_Id { get; set; }
        public long? Section_Id { get; set; }
        public Guid? SupAdmUnit_Id { get; set; }
        public int? School_Id { get; set; }
        public bool Sent { get; set; }
        public EnumBatchSituation Situation { get; set; }
		public Guid? CreatedBy_Id { get; set; }

    }
}
