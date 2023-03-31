using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Repository.Map.Base;

namespace GestaoAvaliacao.Repository.Map
{
    public class BlockChainBlockMap : EntityBaseMap<BlockChainBlock>
    {
        public BlockChainBlockMap()
        {
            ToTable("BlockChainBlock");

            HasRequired(p => p.Block)
                .WithMany(p => p.BlockChainBlocks)
                .HasForeignKey(p => p.Block_Id);

            HasRequired(p => p.BlockChain)
                .WithMany(p => p.BlockChainBlocks)
                .HasForeignKey(p => p.BlockChain_Id);
        }
    }
}
