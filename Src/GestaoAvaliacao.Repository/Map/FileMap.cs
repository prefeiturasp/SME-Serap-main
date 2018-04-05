using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Repository.Map.Base;

namespace GestaoAvaliacao.Repository.Map
{
    class FileMap : EntityBaseMap<File>
    {
        public FileMap()
        {
            ToTable("File");

            Property(p => p.Name)
               .IsRequired()
               .HasMaxLength(500)
               .HasColumnType("varchar");

            Property(p => p.Path)
               .HasMaxLength(500)
               .HasColumnType("varchar");

            Property(p => p.ContentType)
               .IsRequired()
               .HasMaxLength(500)
               .HasColumnType("varchar");

            Property(p => p.OwnerId).IsOptional();
            Property(p => p.OwnerType).IsOptional();
            Property(p => p.ParentOwnerId).IsOptional();
            Property(p => p.OriginalName).IsOptional();
            Property(p => p.CreatedBy_Id).IsOptional();
            Property(p => p.DeletedBy_Id).IsOptional();
            Property(p => p.HorizontalResolution).IsOptional().HasColumnType("float");
            Property(p => p.VerticalResolution).IsOptional().HasColumnType("float");
        }
    }
}
