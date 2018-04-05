using GestaoAvaliacao.Util;
using System;

namespace GestaoAvaliacao.Entities.DTO
{
    public class AnswerSheetLotDTO
	{
		public long Id { get; set; }
        public long Parent_Id { get; set; }
        public long TestCode { get; set; }
		public string Description { get; set; }
		public string TestTypeDescription { get; set; }
		public EnumServiceState StateExecution { get; set; }
		public long FileId { get; set; }
		public string FilePath { get; set; }
        public string FileName { get; set; }
        public EnumAnswerSheetBatchOwner Type { get; set; }
        public string SupAdmUnitName { get; set; }
        public string SchoolName { get; set; }
        public DateTime CreateDate { get; set; }
        public bool AllAdhered { get; set; }
        public int TotalAdherence { get; set; }
    }

    public class AnswerSheetLotHistory
    {
        public string tempFolderSize { get; set; }
        public string mainFolderSize { get; set; }
        public string Owner { get; set; }
    }
}
