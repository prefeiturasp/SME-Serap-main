using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Repository.Map.Base;

namespace GestaoAvaliacao.Repository.Map
{
    class KnowledgeAreaMap : EntityBaseMap<KnowledgeArea>
    {
        public KnowledgeAreaMap()
        {
            ToTable("KnowledgeArea");

            Property(p => p.Description)
               .IsRequired()
               .HasMaxLength(50)
               .HasColumnType("varchar");

            Property(p => p.EntityId)
               .IsRequired();
        }
    }
}
