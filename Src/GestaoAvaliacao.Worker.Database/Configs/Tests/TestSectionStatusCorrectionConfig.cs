using GestaoAvaliacao.Worker.Database.Configs.Base;
using GestaoAvaliacao.Worker.Domain.Entities.Tests;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestaoAvaliacao.Worker.Database.Configs.Tests
{
    public class TestSectionStatusCorrectionConfig : BaseEntityConfig<TestSectionStatusCorrectionEntityWorker>
    {
        protected override void OnConfiguring(EntityTypeBuilder<TestSectionStatusCorrectionEntityWorker> builder)
        {
            builder.Property(x => x.Test_Id).IsRequired();
            builder.Property(x => x.tur_id).IsRequired();
            builder.Property(x => x.StatusCorrection).IsRequired();
        }

        protected override string GetTableName() => "TestSectionStatusCorrection";
    }
}