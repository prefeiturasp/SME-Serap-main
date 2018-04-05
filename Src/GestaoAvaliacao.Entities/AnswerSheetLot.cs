using GestaoAvaliacao.Entities.Base;
using GestaoAvaliacao.Util;
using System;

namespace GestaoAvaliacao.Entities
{
    public class AnswerSheetLot : EntityBase
	{
		public Guid? uad_id { get; set; }
		public int? esc_id { get; set; }
		public virtual Test Test { get; set; }
        public long? Test_Id { get; set; }
        public virtual long TestId
        {
            get
            {
                if (Test_Id != null)
                {
                    return (long)Test_Id;
                }
                else {
                    return 0;
                }
            }
            set
            {
                Test_Id = value;
            }
        }
        public EnumAnswerSheetBatchOwner Type { get; set; }
        public EnumServiceState StateExecution { get; set; }
        public DateTime? RequestDate { get; set; }
        public virtual AnswerSheetLot Parent { get; set; }
        public long? Parent_Id { get; set; }
        public virtual long ParentId
        {
            get
            {
                if (Parent_Id != null)
                {
                    return (long)Parent_Id;
                }
                else {
                    return 0;
                }
            }
            set
            {
                Parent_Id = value;
            }
        }
        public string ExecutionOwner { get; set; }
    }
}
