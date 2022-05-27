using GestaoAvaliacao.Entities.Base;
using GestaoAvaliacao.Entities.Enumerator;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestaoAvaliacao.Entities
{
    public class NumberItemTestTai : EntityBase
    {
        public NumberItemTestTai()
        {

        }

        public NumberItemTestTai(long testId, long itemAplicationTaiId, bool advanceWithoutAnswering, bool backToPreviousItem)
        {
            TestId = testId;
            ItemAplicationTaiId = itemAplicationTaiId;
            CreateDate = UpdateDate = DateTime.Now;
            
            State = Convert.ToByte(EnumState.ativo);
            AdvanceWithoutAnswering = advanceWithoutAnswering;
            BackToPreviousItem = backToPreviousItem;
        }


        [ForeignKey("TestId")]
        public virtual Test Test { get; set; }

        [ForeignKey("ItemAplicationTaiId")]
        public virtual NumberItemTestTai ItemAplicationTai { get; set; }


        [Column("TestId")]
        public virtual long TestId { get; set; }

        [Column("ItemAplicationTaiId")]
        public virtual long ItemAplicationTaiId { get; set; }
        [Column("AdvanceWithoutAnswering")]
        public virtual bool AdvanceWithoutAnswering { get; set; }

        [Column("BackToPreviousItem")]
        public virtual bool BackToPreviousItem { get; set; }
        

    }
}
