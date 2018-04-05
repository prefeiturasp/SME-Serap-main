using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Repository.Map.Base;

namespace GestaoAvaliacao.Repository.Map
{
    public class ModelSkillLevelMap : EntityBaseMap<ModelSkillLevel>
    {
        public ModelSkillLevelMap()
        {
            ToTable("ModelSkillLevel");

            Property(p => p.Description)
               .IsRequired()
               .HasMaxLength(500)
               .HasColumnType("varchar");

            Property(p => p.Level)
               .IsRequired();

            Property(p => p.LastLevel)
            .IsRequired();
        }
    }
}
