using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Repository.Map.Base;

namespace GestaoAvaliacao.Repository.Map
{
    public class BlockKnowledgeAreaMap : EntityBaseMap<BlockKnowledgeArea>
    {
        public BlockKnowledgeAreaMap()
        {
            ToTable("BlockKnowledgeArea");

            Property(p => p.Order)
              .IsRequired();

            HasRequired(p => p.Block)
                .WithMany(p => p.BlockKnowledgeAreas)
                .HasForeignKey(p => p.Block_Id);

            HasRequired(p => p.KnowledgeArea)
                .WithMany(p => p.BlockKnowledgeAreas)
                .HasForeignKey(p => p.KnowledgeArea_Id);
        }
    }
}
