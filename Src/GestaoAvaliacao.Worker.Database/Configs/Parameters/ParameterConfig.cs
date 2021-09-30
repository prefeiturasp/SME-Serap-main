using GestaoAvaliacao.Worker.Database.Configs.Base;
using GestaoAvaliacao.Worker.Domain.Entities.Parameters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestaoAvaliacao.Worker.Database.Configs.Parameters
{
    public class ParameterConfig : BaseEntityConfig<ParameterEntityWorker>
    {
        protected override void OnConfiguring(EntityTypeBuilder<ParameterEntityWorker> builder)
        {
            builder.Property(p => p.Key)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnType("varchar");

            builder.Property(p => p.Value)
                  .IsRequired()
                  .HasMaxLength(8000)
                  .HasColumnType("varchar");

            builder.Property(p => p.Description)
                  .HasMaxLength(200)
                  .HasColumnType("varchar");

            builder.Property(p => p.StartDate)
                .IsRequired();

            builder.Property(p => p.EndDate);
        }

        protected override string GetTableName() => "Parameter";
    }
}