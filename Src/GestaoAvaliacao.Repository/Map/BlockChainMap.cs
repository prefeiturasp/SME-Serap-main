using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Repository.Map.Base;

namespace GestaoAvaliacao.Repository.Map
{
    public class BlockChainMap : EntityBaseMap<BlockChain>
    {
        public BlockChainMap()
        {
            ToTable("BlockChain");

            Property(p => p.Description)
                .IsRequired()
                .HasMaxLength(10)
                .HasColumnType("varchar");

            HasOptional(p => p.Test)
                .WithMany()
                .HasForeignKey(p => p.Test_Id);
        }
    }
}
