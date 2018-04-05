using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Repository.Map.Base;

namespace GestaoAvaliacao.Repository.Map
{
    public class DisciplineMap : EntityBaseMap<Discipline>
    {
        public DisciplineMap()
        {
            ToTable("Discipline");

            Property(p => p.Description)
                .IsRequired()
                .HasMaxLength(500)
                .HasColumnType("varchar");

            Property(p => p.DisciplineTypeId)
              .IsRequired();
                        
        }
    
    }
}
