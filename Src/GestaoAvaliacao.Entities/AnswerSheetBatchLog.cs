using GestaoAvaliacao.Entities.Base;
using GestaoAvaliacao.Util;

namespace GestaoAvaliacao.Entities
{
    public class AnswerSheetBatchLog : EntityBase
    {
        public virtual AnswerSheetBatchFiles AnswerSheetBatchFile { get; set; }
        public long AnswerSheetBatchFile_Id { get; set; }

        public EnumBatchSituation Situation { get; set; }
        public string Description { get; set; }

        public virtual File File { get; set; }
        public long? File_Id { get; set; }
    }
}
