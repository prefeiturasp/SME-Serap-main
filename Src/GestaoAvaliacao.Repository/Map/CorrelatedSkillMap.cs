using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Repository.Map.Base;

namespace GestaoAvaliacao.Repository.Map
{
    public class CorrelatedSkillMap : EntityBaseMap<CorrelatedSkill>
    {
        public CorrelatedSkillMap()
        {
            ToTable("CorrelatedSkill");
                       
        }

    }
}
