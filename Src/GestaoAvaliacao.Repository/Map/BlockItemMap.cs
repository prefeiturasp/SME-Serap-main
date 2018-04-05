using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Repository.Map.Base;

namespace GestaoAvaliacao.Repository.Map
{
    public class BlockItemMap : EntityBaseMap<BlockItem>
    {
        public BlockItemMap()
        {
            ToTable("BlockItem");

            Property(p => p.Order)
              .IsRequired();

            HasRequired(p => p.Block)
                .WithMany(p => p.BlockItems)
                .HasForeignKey(p => p.Block_Id);

            HasRequired(p => p.Item)
                .WithMany(p => p.BlockItems)
                .HasForeignKey(p => p.Item_Id);           
        }
    }
}
