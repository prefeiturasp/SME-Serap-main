using GestaoAvaliacao.Entities.Base;
using System;

namespace GestaoAvaliacao.Entities
{
    [Serializable]
    public class ItemSkill : EntityBase
	{
		public virtual Item Item { get; set; }
		public long Item_Id { get; set; }

		public virtual Skill Skill { get; set; }
		public long Skill_Id { get; set; }

		public virtual Boolean OriginalSkill { get; set; }
	}
}
