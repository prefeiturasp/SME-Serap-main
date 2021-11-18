using GestaoAvaliacao.Entities.Base;
using GestaoAvaliacao.Entities.Enumerator;
using GestaoAvaliacao.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestaoAvaliacao.Entities
{
    [Serializable]
    public class TestContext : EntityBase
	{
        public TestContext()
        {
        }

		public EnumPosition ImagePosition { get; set; }

        [NotMapped]
        public string ImagePositionDescription { get; set; }

        [ForeignKey("Test_Id")]
        public virtual Test Test { get; set; }

        [Column("Test_Id")]
        public long Test_Id { get; set; }
        //public long ItemId { get; set; }
        public string Title { get; set; }
        public string ImagePath { get; set; }
        public string Text { get; set; }
    }
}
