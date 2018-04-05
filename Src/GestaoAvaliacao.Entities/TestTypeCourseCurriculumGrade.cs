using GestaoAvaliacao.Entities.Base;

namespace GestaoAvaliacao.Entities
{
    public class TestTypeCourseCurriculumGrade : EntityBase
    {
        public virtual TestTypeCourse TestTypeCourse { get; set; }
        public long TestTypeCourse_Id { get; set; }
        public int TypeCurriculumGradeId { get; set; }
        public int Ordem { get; set; }
        public int CurriculumGradeId { get; set; }
    }
}
