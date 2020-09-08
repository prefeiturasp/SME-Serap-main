using GestaoAvaliacao.Entities.Base;
using System;

namespace GestaoAvaliacao.Entities
{
    public class TestTypeDeficiency : EntityBase
    {
        public Guid DeficiencyId { get; set; }
        public virtual TestType TestType { get; set; }
        public long TestType_Id { get; set; }
    }
}