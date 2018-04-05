using GestaoAvaliacao.Entities.Base;
using System;

namespace GestaoAvaliacao.Entities
{
    public class ItemType : EntityBase
    {
        public string Description { get; set; }
        public bool UniqueAnswer { get; set; }
        public Guid EntityId { get; set; }
		public bool IsVisibleTestType { get; set; }
		public bool IsDefault { get; set; }
        public int? QuantityAlternative { get; set; }
    }
}
