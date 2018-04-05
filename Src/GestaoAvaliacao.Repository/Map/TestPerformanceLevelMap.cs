using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Repository.Map.Base;

namespace GestaoAvaliacao.Repository.Map
{
    class TestPerformanceLevelMap : EntityBaseMap<TestPerformanceLevel>
    {
        public TestPerformanceLevelMap()
        {
            ToTable("TestPerformanceLevel");
        }
    }
}
