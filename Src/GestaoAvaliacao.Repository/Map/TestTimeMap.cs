using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Repository.Map.Base;

namespace GestaoAvaliacao.Repository.Map
{
    public class TestTimeMap : EntityBaseMap<TestTime>
	{
		public TestTimeMap()
		{
			ToTable("TestTime");

			Property(p => p.Description)
				.IsRequired()
				.HasMaxLength(100)
				.HasColumnType("varchar");
		}
	}
}
