using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Repository.Map.Base;

namespace GestaoAvaliacao.Repository.Map
{
    public class SkillMap : EntityBaseMap<Skill>
    {
       public SkillMap()
       {
           ToTable("Skill");

           Property(p => p.Description)
                .IsRequired()
                .HasMaxLength(500)
                .HasColumnType("varchar");

           Property(p => p.Code)
               .IsRequired()
               .HasMaxLength(500)
               .HasColumnType("varchar");

           Property(p => p.LastLevel)
             .IsRequired();

           HasOptional(p => p.CognitiveCompetence)
               .WithMany()
               .HasForeignKey(p => p.CognitiveCompetence_Id);
              
       }
   }
}
