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

        public NumberItemTestTai(long testId, long itemAplicationTaiId)
        {
            TestId = testId;
            ItemAplicationTaiId = itemAplicationTaiId;
            CreateDate = UpdateDate = DateTime.Now;
            State = Convert.ToByte(EnumState.ativo);
        }


        [ForeignKey("TestId")]
        public virtual Test Test { get; set; }

        [ForeignKey("ItemAplicationTaiId")]
        public virtual NumberItemTestTai ItemAplicationTai { get; set; }


        [Column("TestId")]
        public virtual long TestId { get; set; }

        [Column("ItemAplicationTaiId")]
        public virtual long ItemAplicationTaiId { get; set; }

    }
}
