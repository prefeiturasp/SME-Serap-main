using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Repository.Map.Base;

namespace GestaoAvaliacao.Repository.Map
{
    class FormatTypeMap : EntityBaseMap<FormatType>
    {
        public FormatTypeMap()
        {
            ToTable("FormatType");

            Property(p => p.Description)
               .IsRequired()
               .HasMaxLength(500)
               .HasColumnType("varchar");      
        }
    }
}
