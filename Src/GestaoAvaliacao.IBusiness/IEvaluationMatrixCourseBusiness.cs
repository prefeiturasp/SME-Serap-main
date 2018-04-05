using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Util;
using System.Collections.Generic;

namespace GestaoAvaliacao.IBusiness
{
    public interface IEvaluationMatrixCourseBusiness
    {
        EvaluationMatrixCourse Save(EvaluationMatrixCourse entity);
        EvaluationMatrixCourse Update(long id, EvaluationMatrixCourse entity);
        EvaluationMatrixCourse Get(long id);
        EvaluationMatrixCourse Delete(long id, long evaluationMatrixId);
        IEnumerable<EvaluationMatrixCourse> Load(ref Pager pager);
        IEnumerable<EvaluationMatrixCourse> Search(int evaluationMatrixId, ref Pager pager);
    }
}
