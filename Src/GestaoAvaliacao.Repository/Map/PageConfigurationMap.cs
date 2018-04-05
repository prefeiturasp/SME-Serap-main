using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Repository.Map.Base;

namespace GestaoAvaliacao.Repository.Map
{
    class PageConfigurationMap : EntityBaseMap<PageConfiguration>
    {
        public PageConfigurationMap()
        {
            ToTable("PageConfiguration");

            Property(p => p.Title)
               .IsOptional()
               .HasMaxLength(100)
               .HasColumnType("varchar");

            Property(p => p.Description)
               .IsOptional()
               .HasMaxLength(5000)
               .HasColumnType("varchar");

            Property(p => p.ButtonDescription)
               .IsOptional()
               .HasMaxLength(300)
               .HasColumnType("varchar");

            Property(p => p.Link)
               .IsOptional()
               .HasMaxLength(300)
               .HasColumnType("varchar");

            HasOptional(p => p.FileIllustrativeImage)
                .WithMany()
                .HasForeignKey(p => p.FileIllustrativeImage_Id);

            HasOptional(p => p.FileVideo)
                .WithMany()
                .HasForeignKey(p => p.FileVideo_Id);
        }
    }
}
