using GestaoAvaliacao.Worker.Domain.Base;
using System;

namespace GestaoAvaliacao.Worker.Domain.Entities.Parameters
{
    public class ParameterEntityWorker : EntityWorkerBase
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public string Description { get; set; }
        public Guid EntityId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool? Obligatory { get; set; }
        public bool? Versioning { get; set; }
        public long ParameterPage_Id { get; set; }
        public long ParameterCategory_Id { get; set; }
        public long ParameterType_Id { get; set; }
    }
}