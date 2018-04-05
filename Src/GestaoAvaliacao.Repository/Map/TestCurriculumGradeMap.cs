using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Repository.Map.Base;

namespace GestaoAvaliacao.Repository.Map
{
    public class TestCurriculumGradeMap : EntityBaseMap<TestCurriculumGrade> 
    {
        public TestCurriculumGradeMap()
        {
            ToTable("TestCurriculumGrade");
        }
    }
}
