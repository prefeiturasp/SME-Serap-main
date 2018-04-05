using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Util;
using System.Collections.Generic;

namespace GestaoAvaliacao.IRepository
{
    public interface IEvaluationMatrixCourseRepository
    {
        EvaluationMatrixCourse Save(EvaluationMatrixCourse entity);
        void Update(EvaluationMatrixCourse entity);
        EvaluationMatrixCourse Get(long id);
        IEnumerable<EvaluationMatrixCourse> Load(ref Pager pager);
        void Delete(EvaluationMatrixCourse entity);
        IEnumerable<EvaluationMatrixCourse> Search(int evaluationMatrixId, ref Pager pager);
        bool ExistsItemAndLastCourse(long idCourse, long evaluationMatrixId);
        bool ExistCourse(int courseId, int modalityId, long evaluationMatrixId);
    }
}
