using GestaoAvaliacao.Entities.Base;

namespace GestaoAvaliacao.Entities
{
    public class CorrelatedSkill : EntityBase
    {
        public virtual Skill Skill1 { get; set; }    

        public virtual Skill Skill2 { get; set; }

    }
}
