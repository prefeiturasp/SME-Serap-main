using GestaoAvaliacao.Entities.Base;
using System;

namespace GestaoAvaliacao.Entities
{
    public class AbsenceReason : EntityBase
    {
        public string Description { get; set; }

        public bool AllowRetry { get; set; }

        public Guid EntityId { get; set; }
        public bool IsDefault { get; set; }
    }
}
