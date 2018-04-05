using GestaoAvaliacao.Entities.Base;

namespace GestaoAvaliacao.Entities
{
    public class TestItemLevel : EntityBase
    {
        public int Value { get; set; }

        public int PercentValue { get; set; }

        public Test Test { get; set; }

        public ItemLevel ItemLevel { get; set; }
    }
}
