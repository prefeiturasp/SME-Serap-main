using GestaoAvaliacao.Entities.Base;
using System.Collections.Generic;

namespace GestaoAvaliacao.Entities
{
    public class TestTypeCourse : EntityBase
    {
        public TestTypeCourse()
        {
            this.TestTypeCourseCurriculumGrades = new List<TestTypeCourseCurriculumGrade>();
        }

        public virtual List<TestTypeCourseCurriculumGrade> TestTypeCourseCurriculumGrades { get; set; }

        public int CourseId { get; set; }

        public virtual TestType TestType { get; set; }
        public long TestType_Id { get; set; }
        public int ModalityId { get; set; }
    }
}
