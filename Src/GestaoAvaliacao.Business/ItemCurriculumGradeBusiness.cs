using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.IRepository;

namespace GestaoAvaliacao.Business
{
    public class ItemCurriculumGradeBusiness : IItemCurriculumGradeBusiness
    {
        private readonly IItemCurriculumGradeRepository itemCurriculumGradeRepository;

        public ItemCurriculumGradeBusiness(IItemCurriculumGradeRepository itemCurriculumGradeRepository)
        {
            this.itemCurriculumGradeRepository = itemCurriculumGradeRepository;
        }

        #region Read

        public bool ExistsItemCurriculumGrade(int typeCurriculumGradeId, long evaluationMatrixId)
        {
            return itemCurriculumGradeRepository.ExistsItemCurriculumGrade(typeCurriculumGradeId, evaluationMatrixId);
        }

        #endregion
    }
}
