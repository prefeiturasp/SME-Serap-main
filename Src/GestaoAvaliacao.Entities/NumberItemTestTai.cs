using GestaoAvaliacao.Entities.Base;

namespace GestaoAvaliacao.Entities
{
    public class NumberItemTestTai : EntityBase
    {
        public virtual long TestId { get; set; }
        public virtual long ItemAplicationTaiId { get; set; }
    }
}
