using GestaoAvaliacao.Entities.Base;

namespace GestaoAvaliacao.Entities
{
    public class TestCurriculumGrade : EntityBase
    {
        public long TypeCurriculumGradeId { get; set; }

        public Test Test { get; set; }
    }
}
