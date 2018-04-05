using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Repository.Map.Base;

namespace GestaoAvaliacao.Repository.Map
{
    public class TestTypeCourseCurriculumGradeMap : EntityBaseMap<TestTypeCourseCurriculumGrade>
    {
        public TestTypeCourseCurriculumGradeMap()
        {
            ToTable("TestTypeCourseCurriculumGrade");

            Property(p => p.CurriculumGradeId)
                  .IsRequired();

            HasRequired(p => p.TestTypeCourse)
                .WithMany(p => p.TestTypeCourseCurriculumGrades)
                .HasForeignKey(p => p.TestTypeCourse_Id);
        }
    }
}
