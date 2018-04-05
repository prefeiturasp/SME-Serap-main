using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Repository.Map.Base;

namespace GestaoAvaliacao.Repository.Map
{
    public class TestGroupMap : EntityBaseMap<TestGroup>
    {
        public TestGroupMap()
        {
            ToTable("TestGroup");

            Property(p => p.Description)
               .IsRequired()
               .HasMaxLength(500)
               .HasColumnType("varchar");

            Property(p => p.EntityId)
               .IsRequired();
        }
    }
}
