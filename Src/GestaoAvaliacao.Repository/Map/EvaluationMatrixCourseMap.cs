using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Repository.Map.Base;

namespace GestaoAvaliacao.Repository.Map
{
    public class EvaluationMatrixCourseMap : EntityBaseMap<EvaluationMatrixCourse>
    {
        public EvaluationMatrixCourseMap()
        {
            ToTable("EvaluationMatrixCourse");
          
            Property(p => p.CourseId)
              .IsRequired();

            HasRequired(p => p.EvaluationMatrix)
                .WithMany(p=>p.EvaluationMatrixCourse)
                .HasForeignKey(p => p.EvaluationMatrix_Id);
        }

    }
}
