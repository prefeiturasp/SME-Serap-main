using GestaoAvaliacao.Entities.Base;
using GestaoAvaliacao.Util;
using System;
using System.Collections.Generic;

namespace GestaoAvaliacao.Entities
{
    public class AnswerSheetBatch : EntityBase
	{
		public AnswerSheetBatch()
		{
			this.AnswerSheetBatchFiles = new List<AnswerSheetBatchFiles>();
        }

		public virtual List<AnswerSheetBatchFiles> AnswerSheetBatchFiles { get; set; }


        public string Description { get; set; }
		public virtual Test Test { get; set; }
		public long Test_Id { get; set; }

		public Guid? SupAdmUnit_Id { get; set; }
		public int? School_Id { get; set; }
		public long? Section_Id { get; set; }
		public Guid? CreatedBy_Id { get; set; }
		public EnumAnswerSheetBatchType BatchType { get; set; }
		public EnumAnswerSheetBatchOwner OwnerEntity { get; set; }
		public EnumBatchProcessing Processing { get; set; }
	}
}
