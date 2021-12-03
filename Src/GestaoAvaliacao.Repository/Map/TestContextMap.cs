using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Repository.Map.Base;

namespace GestaoAvaliacao.Repository.Map
{
    public class TestContextMap : EntityBaseMap<TestContext>
	{
		public TestContextMap()
		{
			ToTable("TestContext");

            Property(p => p.Title)
                .HasMaxLength(100)
                .HasColumnType("varchar");

            Property(p => p.Text)
                .HasColumnType("varchar(max)");
        }
    }
}
