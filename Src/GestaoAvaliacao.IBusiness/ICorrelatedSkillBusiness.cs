using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Util;
using System.Collections.Generic;

namespace GestaoAvaliacao.IBusiness
{
    public interface ICorrelatedSkillBusiness
    {
        CorrelatedSkill Save(CorrelatedSkill entity);
        CorrelatedSkill Get(long id);
        CorrelatedSkill Delete(long id);
        List<CorrelatedSkillByEvaluationMatrix> LoadList(long MatrizId, ref Pager pager);
        List<Skill> LoadCorrelatedSkills(long skillId);
        bool ExistsCorrelated(CorrelatedSkill entity);
    }
}
