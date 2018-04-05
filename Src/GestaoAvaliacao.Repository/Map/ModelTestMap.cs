using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Repository.Map.Base;

namespace GestaoAvaliacao.Repository.Map
{
    public class ModelTestMap : EntityBaseMap<ModelTest>
	{
		public ModelTestMap()
		{
			ToTable("ModelTest");

			Property(p => p.MessageHeader)
				.HasColumnType("text");

			Property(p => p.MessageFooter)
				.HasColumnType("text");

			Property(p => p.CoverPageText)
				.HasColumnType("text");

			Property(p => p.HeaderHtml)
				.HasColumnType("text");

			Property(p => p.FooterHtml)
				.HasColumnType("text");

			Property(p => p.StudentInformationHtml)
				.HasColumnType("text");

			HasOptional(p => p.LogoFooter)
				.WithMany()
				.HasForeignKey(p => p.FileFooter_Id);

			HasOptional(p => p.LogoHeader)
				.WithMany()
				.HasForeignKey(p => p.FileHeader_Id);


			Ignore(p => p.Files);
		}
	}
}
