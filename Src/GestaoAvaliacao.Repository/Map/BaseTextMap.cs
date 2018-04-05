using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Repository.Map.Base;

namespace GestaoAvaliacao.Repository.Map
{
    public class BaseTextMap : EntityBaseMap<BaseText>
    {
        public BaseTextMap()
        {
            ToTable("BaseText");

            Property(p => p.Description)
                .HasColumnType("varchar(MAX)");

            Property(p => p.Source)
               .HasColumnType("varchar(MAX)");

            Property(p => p.InitialOrientation)
                .IsOptional()
                .HasMaxLength(500);
            Property(p => p.InitialStatement)
                .IsOptional()
                .HasMaxLength(300);
            Property(p => p.BaseTextOrientation)
                .IsOptional()
                .HasMaxLength(300);
            Property(p => p.NarrationInitialStatement).IsOptional();
            Property(p => p.StudentBaseText).IsOptional();
            Property(p => p.NarrationStudentBaseText).IsOptional();
        }
    }
}
