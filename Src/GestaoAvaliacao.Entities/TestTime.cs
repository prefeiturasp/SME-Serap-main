using GestaoAvaliacao.Entities.Base;

namespace GestaoAvaliacao.Entities
{
    public class TestTime : EntityBase
    {
        public virtual string Description { get; set; }
        public virtual int Segundos { get; set; }
    }
}
