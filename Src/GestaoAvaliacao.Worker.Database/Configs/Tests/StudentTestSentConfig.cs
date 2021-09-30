using GestaoAvaliacao.Worker.Database.Configs.Base;
using GestaoAvaliacao.Worker.Domain.Entities.Tests;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestaoAvaliacao.Worker.Database.Configs.Tests
{
    public class StudentTestSentConfig : BaseEntityConfig<StudentTestSentEntityWorker>
    {
        protected override void OnConfiguring(EntityTypeBuilder<StudentTestSentEntityWorker> builder)
        {
            builder.Property(x => x.TestId).IsRequired();
            builder.Property(x => x.TurId).IsRequired();
            builder.Property(x => x.AluId).IsRequired();
            builder.Property(x => x.EntId).IsRequired();
            builder.Property(x => x.Vision).IsRequired().HasConversion<short>();
            builder.Property(x => x.Situation).IsRequired().HasConversion<short>();
        }

        protected override string GetTableName() => "StudentTestSent";
    }
}