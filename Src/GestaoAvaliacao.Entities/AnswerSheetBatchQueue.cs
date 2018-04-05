using GestaoAvaliacao.Entities.Base;
using GestaoAvaliacao.Util;
using System;
using System.Collections.Generic;

namespace GestaoAvaliacao.Entities
{
    public class AnswerSheetBatchQueue : EntityBase
    {
        public AnswerSheetBatchQueue()
        {
            this.AnswerSheetBatchFiles = new List<AnswerSheetBatchFiles>();
        }
        public virtual File File { get; set; }
        public long File_Id { get; set; }

        public virtual AnswerSheetBatch AnswerSheetBatch { get; set; }
        public long? AnswerSheetBatch_Id { get; set; }
        public Guid? SupAdmUnit_Id { get; set; }
        public int? School_Id { get; set; }
        public Guid EntityId { get; set; }
        public int? CountFiles { get; set; }
        public EnumBatchQueueSituation Situation { get; set; }
        public string Description { get; set; }
        public Guid CreatedBy_Id { get; set; }
        public virtual List<AnswerSheetBatchFiles> AnswerSheetBatchFiles { get; set; }
    }
}
