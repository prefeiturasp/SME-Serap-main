using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Repository.Map.Base;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;

namespace GestaoAvaliacao.Repository.Map
{
    public class ItemMap : EntityBaseMap<Item>
	{
		public ItemMap()
		{
			ToTable("Item");

			HasRequired(p => p.ItemSituation);
			HasRequired(p => p.ItemType);
			HasRequired(p => p.EvaluationMatrix);

            Property(p => p.ItemCode)
                .IsRequired()
                .HasColumnType("varchar")
                .HasMaxLength(32);

            Property(x => x.ItemCodeVersion).IsRequired();
            Property(x => x.ItemVersion).IsRequired();

            Property(x => x.ItemCodeVersion)
                .IsRequired().HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("UN_Item_ItemCodeVersion_ItemVersion", 1) { IsUnique = true }));

            Property(p => p.ItemVersion)
                .IsRequired().HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("UN_Item_ItemCodeVersion_ItemVersion", 2) { IsUnique = true }));

            Property(p => p.LastVersion)
			   .IsOptional();

			Property(p => p.IsRestrict)
				.IsRequired();

			Property(p => p.Statement)
			   .HasColumnType("varchar(MAX)");

			Property(p => p.Keywords)
			   .HasColumnType("varchar(MAX)");

			Property(p => p.Tips)
			   .HasColumnType("varchar(MAX)");

			Property(p => p.TRIDiscrimination).HasPrecision(9, 3).IsOptional();

			Property(p => p.TRIDifficulty).HasPrecision(9, 3).IsOptional();

			Property(p => p.TRICasualSetting).HasPrecision(9, 3).IsOptional();

			Property(p => p.descriptorSentence)
			   .HasMaxLength(170)
			   .HasColumnType("varchar");



			HasOptional(p => p.BaseText)
				.WithMany()
				.HasForeignKey(p => p.BaseText_Id);

			HasRequired(p => p.EvaluationMatrix)
				.WithMany()
				.HasForeignKey(p => p.EvaluationMatrix_Id);

			HasOptional(p => p.ItemLevel)
				.WithMany()
				.HasForeignKey(p => p.ItemLevel_Id);

			HasRequired(p => p.ItemSituation)
				.WithMany()
				.HasForeignKey(p => p.ItemSituation_Id);

			HasRequired(p => p.ItemType)
				.WithMany()
				.HasForeignKey(p => p.ItemType_Id);

            Property(p => p.ItemNarrated).IsOptional();
            Property(p => p.StudentStatement).IsOptional();
            Property(p => p.NarrationStudentStatement).IsOptional();
            Property(p => p.NarrationAlternatives).IsOptional();

            HasOptional(p => p.KnowledgeArea)
                .WithMany()
                .HasForeignKey(p => p.KnowledgeArea_Id);

            HasOptional(p => p.SubSubject)
                .WithMany()
                .HasForeignKey(p => p.SubSubject_Id);
        }
	}
}
