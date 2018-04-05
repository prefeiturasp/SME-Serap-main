using GestaoAvaliacao.Entities.Base;

namespace GestaoAvaliacao.Entities
{
    public class EvaluationMatrixCourseCurriculumGrade : EntityBase
    {
        public virtual EvaluationMatrixCourse EvaluationMatrixCourse { get; set; }
        public long EvaluationMatrixCourse_Id { get; set; }

        public int CurriculumGradeId { get; set; }
        public int TypeCurriculumGradeId { get; set; }
        public int Ordem { get; set; }
    }
}
