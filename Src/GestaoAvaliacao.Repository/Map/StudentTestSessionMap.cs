using GestaoAvaliacao.Entities.StudentTestAccoplishments;
using GestaoAvaliacao.Repository.Map.Base;

namespace GestaoAvaliacao.Repository.Map
{
    public class StudentTestSessionMap : EntityBaseMap<StudentTestSession>
    {
        public StudentTestSessionMap()
        {
            ToTable(nameof(StudentTestSession));

            Property(x => x.ConnectionId).IsRequired();

            HasRequired(x => x.StudentTestAccoplishment)
                .WithMany(x => x.Sessions)
                .HasForeignKey(x => x.StudentTestAccoplishment_Id);

            Property(x => x.Situation).IsRequired();
            Property(x => x.StartDate).IsRequired();
            Property(x => x.EndDate).IsOptional();
        }
    }
}