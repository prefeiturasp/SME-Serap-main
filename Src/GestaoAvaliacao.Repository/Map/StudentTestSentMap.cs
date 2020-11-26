using GestaoAvaliacao.Entities.StudentsTestSent;
using GestaoAvaliacao.Repository.Map.Base;

namespace GestaoAvaliacao.Repository.Map
{
    public class StudentTestSentMap : EntityBaseMap<StudentTestSent>
    {
        public StudentTestSentMap()
        {
            ToTable(nameof(StudentTestSent));
            Property(x => x.TestId).IsRequired();
            Property(x => x.TurId).IsRequired();
            Property(x => x.AluId).IsRequired();
            Property(x => x.EntId).IsRequired();
            Property(x => x.Vision).IsRequired();
            Property(x => x.Situation).IsRequired();
        }
    }
}