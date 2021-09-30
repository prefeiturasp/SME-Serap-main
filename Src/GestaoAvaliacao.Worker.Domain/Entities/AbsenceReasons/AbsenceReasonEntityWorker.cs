using System;

namespace GestaoAvaliacao.Worker.Domain.Entities.AbsenceReasons
{
    public class AbsenceReasonEntityWorker
    {
        public string Description { get; set; }

        public bool AllowRetry { get; set; }

        public Guid EntityId { get; set; }
        public bool IsDefault { get; set; }
    }
}