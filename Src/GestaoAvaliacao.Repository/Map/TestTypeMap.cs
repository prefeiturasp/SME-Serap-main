using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Repository.Map.Base;

namespace GestaoAvaliacao.Repository.Map
{
    public class TestTypeMap : EntityBaseMap<TestType>
	{
		public TestTypeMap()
		{
			ToTable("TestType");

			Property(p => p.Description)
				.IsRequired()
				.HasMaxLength(500)
				.HasColumnType("varchar");

			Property(p => p.EntityId)
			  .IsRequired();

			HasOptional(p => p.FormatType)
				.WithMany()
				.HasForeignKey(p => p.FormatType_Id);

			HasOptional(p => p.ItemType)
				.WithMany()
				.HasForeignKey(p => p.ItemType_Id);

			HasOptional(p => p.ModelTest)
				.WithMany()
				.HasForeignKey(p => p.ModelTest_Id);
		}
	}
}
