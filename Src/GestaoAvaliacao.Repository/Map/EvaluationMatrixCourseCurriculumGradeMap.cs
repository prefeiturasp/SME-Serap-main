using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Repository.Map.Base;

namespace GestaoAvaliacao.Repository.Map
{
    public class EvaluationMatrixCourseCurriculumGradeMap : EntityBaseMap<EvaluationMatrixCourseCurriculumGrade>
    {
        public EvaluationMatrixCourseCurriculumGradeMap()
        {
            ToTable("EvaluationMatrixCourseCurriculumGrade");
            
            Property(p => p.CurriculumGradeId)
              .IsRequired();

            HasRequired(p => p.EvaluationMatrixCourse)
                .WithMany(p => p.EvaluationMatrixCourseCurriculumGrades)
                .HasForeignKey(p => p.EvaluationMatrixCourse_Id);
        }
    }
}
