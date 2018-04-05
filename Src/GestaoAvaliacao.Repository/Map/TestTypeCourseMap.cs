using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Repository.Map.Base;

namespace GestaoAvaliacao.Repository.Map
{
    public class TestTypeCourseMap : EntityBaseMap<TestTypeCourse>
    {
        public TestTypeCourseMap()
        {
            ToTable("TestTypeCourse");
            
            Property(p => p.CourseId)
              .IsRequired();

            Property(p => p.ModalityId)
              .IsRequired();

            HasRequired(p => p.TestType)
                .WithMany(p => p.TestTypeCourses)
                .HasForeignKey(p => p.TestType_Id);
        }
    }
}
