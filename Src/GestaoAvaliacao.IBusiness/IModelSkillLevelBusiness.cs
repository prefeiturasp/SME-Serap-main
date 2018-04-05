using GestaoAvaliacao.Entities;
using System.Collections.Generic;

namespace GestaoAvaliacao.IBusiness
{
    public interface IModelSkillLevelBusiness
    {
        ModelSkillLevel Get(long id);

        IEnumerable<ModelSkillLevel> Load(long modelEvatuationMatrixId);

        ModelSkillLevel GetByLevel(int level, long modelEvatuationMatrixId);

        IEnumerable<ModelSkillLevel> GetByMatrixModel(long idMatrix);

        IEnumerable<ModelSkillLevel> GetById(long ModelSkillLevelId);
    }
}
