using GestaoAvaliacao.Entities.StudentsTestSent;
using GestaoAvaliacao.Worker.Database.Configs.Base;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestaoAvaliacao.Worker.Database.Configs.Test
{
    public class StudentTestSentConfig : BaseEntityConfig<StudentTestSent>
    {
        protected override void OnConfiguring(EntityTypeBuilder<StudentTestSent> builder)
        {
            builder.Property(x => x.TestId).IsRequired();
            builder.Property(x => x.TurId).IsRequired();
            builder.Property(x => x.AluId).IsRequired();
            builder.Property(x => x.EntId).IsRequired();
            builder.Property(x => x.Vision).IsRequired();
            builder.Property(x => x.Situation).IsRequired();
        }
    }
}