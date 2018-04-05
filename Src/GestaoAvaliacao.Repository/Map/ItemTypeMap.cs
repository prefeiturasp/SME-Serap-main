using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Repository.Map.Base;

namespace GestaoAvaliacao.Repository.Map
{
    public class ItemTypeMap : EntityBaseMap<ItemType>
    {
        public ItemTypeMap()
        {
            ToTable("ItemType");

            Property(p => p.Description)
                 .IsRequired()
                 .HasMaxLength(500)
                 .HasColumnType("varchar");

            Property(p => p.UniqueAnswer)
              .IsRequired();

            Property(p => p.QuantityAlternative).IsOptional();               
        }    
    }
}
