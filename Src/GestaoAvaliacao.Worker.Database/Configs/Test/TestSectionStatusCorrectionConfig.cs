using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Worker.Database.Configs.Base;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestaoAvaliacao.Worker.Database.Configs.Test
{
    public class TestSectionStatusCorrectionConfig : BaseEntityConfig<TestSectionStatusCorrection>
    {
        protected override void OnConfiguring(EntityTypeBuilder<TestSectionStatusCorrection> builder)
        {
            builder.Ignore(x => x.Test);
        }
    }
}