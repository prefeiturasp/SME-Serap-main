using GestaoEscolar.Entities;
using System.Data.Entity.ModelConfiguration;

namespace GestaoEscolar.Repository.Map
{
    public class ACA_TipoNivelEnsinoMap : EntityTypeConfiguration<ACA_TipoNivelEnsino>
	{
        public ACA_TipoNivelEnsinoMap()
		{
            ToTable("ACA_TipoNivelEnsino");

            HasKey(p => p.tne_id);

			Property(p => p.tne_nome)
						.IsRequired()
						.HasMaxLength(100)
						.HasColumnType("varchar");
            Property(p => p.tne_ordem).IsRequired();
            Property(p => p.tne_dataCriacao).IsRequired();
            Property(p => p.tne_dataAlteracao).IsRequired();
            Property(p => p.tne_situacao)
                        .IsRequired()
                        .HasColumnType("tinyint");
		}
	}
}
