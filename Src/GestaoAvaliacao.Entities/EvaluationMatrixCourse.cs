using GestaoAvaliacao.Entities.Base;
using System.Collections.Generic;

namespace GestaoAvaliacao.Entities
{
    public class EvaluationMatrixCourse : EntityBase
    {
        public EvaluationMatrixCourse()
        {
            this.EvaluationMatrixCourseCurriculumGrades = new List<EvaluationMatrixCourseCurriculumGrade>();            
        }

        public virtual List<EvaluationMatrixCourseCurriculumGrade> EvaluationMatrixCourseCurriculumGrades { get; set; }

        public virtual EvaluationMatrix EvaluationMatrix { get; set; }
        public long EvaluationMatrix_Id { get; set; }

        public int CourseId { get; set; }
        public int TypeLevelEducationId { get; set; }
        public int ModalityId { get; set; }
    }
}
