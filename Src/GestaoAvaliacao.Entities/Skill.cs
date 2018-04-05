using GestaoAvaliacao.Entities.Base;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestaoAvaliacao.Entities
{
    public class Skill : EntityBase
	{
		public Skill()
		{
			ItemSkills = new List<ItemSkill>();    
		}

		
		public virtual Skill Parent { get; set; }
		public virtual string Description { get; set; }
		public virtual string Code { get; set; }
		public virtual bool LastLevel { get; set; }
		public virtual EvaluationMatrix EvaluationMatrix { get; set; }
		public virtual ModelSkillLevel ModelSkillLevel { get; set; }

		public virtual CognitiveCompetence CognitiveCompetence { get; set; }
        public long? CognitiveCompetence_Id { get; set; }

        [NotMapped]
        public virtual long ParentId { get; set; }

        public virtual List<ItemSkill> ItemSkills { get; set; }
	}
}
