using GestaoAvaliacao.Entities.Base;

namespace GestaoAvaliacao.Entities
{
    public class TestTypeItemLevel : EntityBase
    {
        public virtual TestType TestType { get; set; }
        public virtual ItemLevel ItemLevel { get; set; }
        public virtual int? Value { get; set; }
    }
}
