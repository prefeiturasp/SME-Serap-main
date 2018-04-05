using GestaoAvaliacao.Entities.Base;
using System;

namespace GestaoAvaliacao.Entities
{
    public class PerformanceLevel : EntityBase
    {
        public virtual string Description { get; set; }
        public virtual string Code { get; set; }
        public virtual Guid EntityId { get; set; }
    }
}
