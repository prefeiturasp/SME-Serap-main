using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Repository.Map.Base;

namespace GestaoAvaliacao.Repository.Map
{
    public class ItemLevelMap : EntityBaseMap<ItemLevel>
    {
        public ItemLevelMap()
        {
            ToTable("ItemLevel");

            Property(p => p.Description)
                .IsRequired()
                .HasMaxLength(500)
                .HasColumnType("varchar");

            Property(p => p.Value)
              .IsRequired();
               
           
        }
    }
}
