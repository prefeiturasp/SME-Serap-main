using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Repository.Map.Base;

namespace GestaoAvaliacao.Repository.Map
{
    public class AlternativeMap : EntityBaseMap<Alternative>
    {
        public AlternativeMap()
        {
            ToTable("Alternative");

            HasRequired(p => p.Item);

            Property(p => p.Description)
                .HasColumnType("varchar(MAX)");

            Property(p => p.Correct)
              .IsOptional();

            Property(p => p.Order)
              .IsOptional();

            Property(p => p.Justificative)
                .HasColumnType("varchar(MAX)");

            Property(p => p.Numeration)
                .HasMaxLength(10)
                .HasColumnType("varchar");

            Property(p => p.TCTDiscrimination).HasPrecision(9, 3).IsOptional();

            Property(p => p.TCTDificulty).HasPrecision(9, 3).IsOptional();

            Property(p => p.TCTBiserialCoefficient).HasPrecision(9, 3).IsOptional();
        }
    }
}
