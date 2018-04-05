using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Util;
using System.Collections.Generic;

namespace GestaoAvaliacao.IRepository
{
    public interface IItemCurriculumGradeRepository
    {
        ItemCurriculumGrade Save(ItemCurriculumGrade entity);
        void Update(ItemCurriculumGrade entity);
        ItemCurriculumGrade Get(long id);
        IEnumerable<ItemCurriculumGrade> Load(ref Pager pager);
        void Delete(long id);
        bool ExistsItemCurriculumGrade(int typeCurriculumGradeId, long evaluationMatrixId);
    }
}
