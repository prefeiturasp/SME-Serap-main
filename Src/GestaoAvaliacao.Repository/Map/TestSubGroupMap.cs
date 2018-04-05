using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Repository.Map.Base;

namespace GestaoAvaliacao.Repository.Map
{
    class TestSubGroupMap : EntityBaseMap<TestSubGroup>
    {
        public TestSubGroupMap()
        {
            ToTable("TestSubGroup");

            Property(p => p.Description)
               .IsRequired()
               .HasMaxLength(500)
               .HasColumnType("varchar");
        }
    }
}
