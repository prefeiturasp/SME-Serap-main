using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Repository.Map.Base;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;

namespace GestaoAvaliacao.Repository.Map
{
    public class ItemSituationMap : EntityBaseMap<ItemSituation>
    {
        public ItemSituationMap()
        {
            ToTable("ItemSituation");

            Property(p => p.Description)
               .IsRequired()
               .HasMaxLength(500)
               .HasColumnType("varchar");

            Property(p => p.Description)
                .IsRequired().HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("UN_ItemSituation_Description_EntityId", 1) { IsUnique = true }));

            Property(p => p.EntityId)
                .IsRequired().HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("UN_ItemSituation_Description_EntityId", 2) { IsUnique = true }));
        }
    }
}
