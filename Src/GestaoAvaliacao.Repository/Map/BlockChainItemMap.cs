using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Repository.Map.Base;

namespace GestaoAvaliacao.Repository.Map
{
    public class BlockChainItemMap : EntityBaseMap<BlockChainItem>
    {
        public BlockChainItemMap()
        {
            ToTable("BlockChainItem");

            Property(p => p.Order)
                .IsRequired();

            HasRequired(p => p.BlockChain)
                .WithMany(p => p.BlockChainItems)
                .HasForeignKey(p => p.BlockChain_Id);

            HasRequired(p => p.Item)
                .WithMany(p => p.BlockChainItems)
                .HasForeignKey(p => p.Item_Id);
        }
    }
}
