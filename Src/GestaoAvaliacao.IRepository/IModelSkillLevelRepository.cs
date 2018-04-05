using GestaoAvaliacao.Entities;
using System.Collections.Generic;

namespace GestaoAvaliacao.IRepository
{
    public interface IModelSkillLevelRepository
    {
        IEnumerable<ModelSkillLevel> GetByMatrixModel(long idMatrix);
        ModelSkillLevel Get(long id);
        IEnumerable<ModelSkillLevel> Load(long modelEvatuationMatrixId);
        ModelSkillLevel GetByLevel(int level, long modelEvatuationMatrixId);

        IEnumerable<ModelSkillLevel> GetById(long ModelSkillLevelId);
    }
}
