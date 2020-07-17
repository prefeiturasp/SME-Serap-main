using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Repository.Map.Base;

namespace GestaoAvaliacao.Repository.Map
{
    public class TestTypeDeficiencyMap : EntityBaseMap<TestTypeDeficiency>
    {
        public TestTypeDeficiencyMap()
        {
            ToTable(nameof(TestTypeDeficiency));

            Property(p => p.DeficiencyId)
              .IsRequired();

            HasRequired(p => p.TestType);
        }
    }
}