using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Repository.Map.Base;

namespace GestaoAvaliacao.Repository.Map
{
    public class TestMap : EntityBaseMap<Test>
	{
		public TestMap()
		{
			ToTable("Test");

			Property(p => p.Description)
				.IsRequired()
				.HasMaxLength(60)
				.HasColumnType("varchar");

			HasRequired(t => t.TestType)
				.WithMany()
				.HasForeignKey(t => t.TestType_Id);

            HasOptional(t => t.Discipline)
                .WithMany()
                .HasForeignKey(t => t.Discipline_Id);

            HasRequired(t => t.FormatType)
                .WithMany()
                .HasForeignKey(t => t.FormatType_Id);

            HasOptional(t => t.TestSubGroup)
                .WithMany()
                .HasForeignKey(t => t.TestSubGroup_Id);
        }
    }
}
