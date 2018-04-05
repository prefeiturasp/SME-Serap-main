using GestaoAvaliacao.Entities.Base;
using System;

namespace GestaoAvaliacao.Entities
{
    public class ItemSituation : EntityBase
    {
        public virtual string Description { get; set; }
        public virtual bool AllowVersion { get; set; }
        public virtual Guid EntityId { get; set; }
    }
}
