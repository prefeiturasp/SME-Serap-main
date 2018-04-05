using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Repository.Map.Base;

namespace GestaoAvaliacao.Repository.Map
{
    public class PerformanceLevelMap : EntityBaseMap<PerformanceLevel>
    {
        public PerformanceLevelMap()
        {
            ToTable("PerformanceLevel");

            Property(p => p.Description)
                .IsRequired()
                .HasMaxLength(50)
                .HasColumnType("varchar");

            Property(p => p.Code)
              .IsRequired()
              .HasMaxLength(15)
              .HasColumnType("varchar");            
        }
    }
}
