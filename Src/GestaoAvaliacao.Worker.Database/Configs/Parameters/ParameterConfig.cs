using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Worker.Database.Configs.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestaoAvaliacao.Worker.Database.Configs.Parameters
{
    public class ParameterConfig : BaseEntityConfig<Parameter>
    {
        protected override void OnConfiguring(EntityTypeBuilder<Parameter> builder)
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

            builder.Ignore(p => p.ParameterPage);
            builder.Ignore(p => p.ParameterCategory);
            builder.Ignore(p => p.ParameterType);
        }
    }
}