using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Repository.Map.Base;

namespace GestaoAvaliacao.Repository.Map
{
    public class BlockMap : EntityBaseMap<Block>
    {
        public BlockMap()
        {
            ToTable("Block");

            Property(p => p.Description)
                .IsRequired()
                .HasMaxLength(10)
                .HasColumnType("varchar");

            HasOptional(p => p.Booklet)
                .WithMany(p => p.Blocks)
                .HasForeignKey(p => p.Booklet_Id);

            HasOptional(p => p.Test)
                .WithMany(p => p.Blocks)
                .HasForeignKey(p => p.Test_Id);
        }
    }
}
