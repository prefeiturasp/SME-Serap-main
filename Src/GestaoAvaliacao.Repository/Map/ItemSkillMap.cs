using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Repository.Map.Base;

namespace GestaoAvaliacao.Repository.Map
{
    public class ItemSkillMap : EntityBaseMap<ItemSkill>
	{
		public ItemSkillMap()
		{
			ToTable("ItemSkill");

			Property(p => p.OriginalSkill)
				.IsOptional();

			HasRequired(p => p.Skill)
				.WithMany(s => s.ItemSkills)
				.HasForeignKey(p => p.Skill_Id);

			HasRequired(p => p.Item)
				.WithMany(i => i.ItemSkills)
				.HasForeignKey(p => p.Item_Id);
		}
	}
}
