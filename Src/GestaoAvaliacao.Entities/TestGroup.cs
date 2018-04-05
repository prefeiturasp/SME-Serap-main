using GestaoAvaliacao.Entities.Base;
using System;
using System.Collections.Generic;

namespace GestaoAvaliacao.Entities
{
    public class TestGroup : EntityBase
    {
        public TestGroup()
        {
            TestSubGroups = new List<TestSubGroup>();           
        }

        public virtual string Description { get; set; }

        public virtual Guid EntityId { get; set; }

        public virtual List<TestSubGroup> TestSubGroups { get; set; }
    }
}
