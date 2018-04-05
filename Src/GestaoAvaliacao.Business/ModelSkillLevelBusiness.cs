using GestaoAvaliacao.Entities;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.IRepository;
using System.Collections.Generic;

namespace GestaoAvaliacao.Business
{
    public class ModelSkillLevelBusiness : IModelSkillLevelBusiness
    {
        private readonly IModelSkillLevelRepository modelSkillLevelRepository;

        public ModelSkillLevelBusiness(IModelSkillLevelRepository modelSkillLevelRepository)
        {
            this.modelSkillLevelRepository = modelSkillLevelRepository;
        }

        #region Read

        public ModelSkillLevel Get(long id)
        {
            return modelSkillLevelRepository.Get(id);
        }

        public IEnumerable<ModelSkillLevel> Load(long modelEvatuationMatrixId)
        {
            return modelSkillLevelRepository.Load(modelEvatuationMatrixId);
        }

        public ModelSkillLevel GetByLevel(int level, long modelEvatuationMatrixId)
        {
            return modelSkillLevelRepository.GetByLevel(level, modelEvatuationMatrixId);
        }

        public IEnumerable<ModelSkillLevel> GetByMatrixModel(long idMatrix)
        {
            return modelSkillLevelRepository.GetByMatrixModel(idMatrix);
        }
        
        public IEnumerable<ModelSkillLevel> GetById(long modelEvatuationMatrixId)
        {
            return modelSkillLevelRepository.GetById(modelEvatuationMatrixId);
        }

        #endregion
    }
}
