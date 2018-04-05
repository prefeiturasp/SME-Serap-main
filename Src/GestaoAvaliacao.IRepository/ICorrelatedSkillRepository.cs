using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Util;
using System.Collections.Generic;

namespace GestaoAvaliacao.IRepository
{
    public interface ICorrelatedSkillRepository
    {
        CorrelatedSkill Save(CorrelatedSkill entity);
        CorrelatedSkill Get(long id);
        void Delete(long id);
        List<CorrelatedSkillByEvaluationMatrix> LoadList(long MatrizId, ref Pager pager);
        List<Skill> LoadCorrelatedSkills(long skillId);
        bool ExistsCorrelated(CorrelatedSkill entity);
    }
}
