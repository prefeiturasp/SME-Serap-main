using GestaoAvaliacao.Entities.StudentTestAccoplishments;
using GestaoAvaliacao.Repository.Map.Base;

namespace GestaoAvaliacao.Repository.Map
{
    public class StudentTestAccoplishmentMap : EntityBaseMap<StudentTestAccoplishment>
    {
        public StudentTestAccoplishmentMap() : base()
        {
            ToTable(nameof(StudentTestAccoplishment));

            Property(x => x.AluId).IsRequired();
            Property(x => x.TurId).IsRequired();
            HasRequired(x => x.Test)
                .WithMany()
                .HasForeignKey(x => x.Test_Id);

            Property(x => x.Situation).IsRequired();
            Property(x => x.StartDate).IsRequired();
            Property(x => x.EndDate).IsOptional();
        }
    }
}