using GestaoAvaliacao.Entities.Base;

namespace GestaoAvaliacao.Entities
{
    public class TestPerformanceLevel : EntityBase
    {
        public PerformanceLevel PerformanceLevel { get; set; }

        public int Order { get; set; }

        public int? Value1 { get; set; }

        public int? Value2 { get; set; }

        public Test Test { get; set; }
    }
}
