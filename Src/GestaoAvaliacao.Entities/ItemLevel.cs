using GestaoAvaliacao.Entities.Base;
using System;
using System.Collections.Generic;

namespace GestaoAvaliacao.Entities
{
    public class ItemLevel : EntityBase
    {
        public ItemLevel(){
            TestTypeItemLevels = new List<TestTypeItemLevel>();
        }

        public virtual string Description { get; set; }

        public virtual int Value { get; set; }
        public virtual Guid EntityId { get; set; }
        public virtual List<TestTypeItemLevel> TestTypeItemLevels {get; set;}
    }
}
