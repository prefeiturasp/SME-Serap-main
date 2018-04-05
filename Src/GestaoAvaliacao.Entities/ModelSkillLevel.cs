using GestaoAvaliacao.Entities.Base;
using System.Collections.Generic;

namespace GestaoAvaliacao.Entities
{
    public class ModelSkillLevel : EntityBase
    {
        public ModelSkillLevel()
        {
            Skills = new List<Skill>();
        }

        public virtual string Description { get; set; }

        public virtual ModelEvaluationMatrix ModelEvaluationMatrix { get; set; }

        public virtual int Level { get; set; }

        public virtual List<Skill> Skills { get; set; }

        public virtual bool LastLevel { get; set; }

    }
}
