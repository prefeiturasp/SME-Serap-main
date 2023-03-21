using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Repository.Map.Base;

namespace GestaoAvaliacao.Repository.Map
{
    public class BlockChainBlockMap : EntityBaseMap<BlockChainBlock>
    {
        public BlockChainBlockMap()
        {
            ToTable("BlockChainBlock");

            Property(p => p.Description)
                .IsRequired()
                .HasMaxLength(10)
                .HasColumnType("varchar");

            HasOptional(p => p.BlockChain)
                .WithMany()
                .HasForeignKey(p => p.BlockChain_Id);
        }
    }
}
