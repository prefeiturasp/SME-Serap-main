using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Repository.Map.Base;

namespace GestaoAvaliacao.Repository.Map
{
    public class TestItemLevelMap : EntityBaseMap<TestItemLevel>
    {
        public TestItemLevelMap()
        {
            ToTable("TestItemLevel");

            Property(p => p.Value)
               .IsOptional();

            Property(p => p.PercentValue)
               .IsOptional();
        }

    }
}
