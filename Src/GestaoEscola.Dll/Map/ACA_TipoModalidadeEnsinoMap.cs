using GestaoEscolar.Entities;
using System.Data.Entity.ModelConfiguration;

namespace GestaoEscolar.Repository.Map
{
    public class ACA_TipoModalidadeEnsinoMap : EntityTypeConfiguration<ACA_TipoModalidadeEnsino>
	{
        public ACA_TipoModalidadeEnsinoMap()
		{
            ToTable("ACA_TipoModalidadeEnsino");

            HasKey(p => p.tme_id);

			Property(p => p.tme_nome)
						.IsRequired()
						.HasMaxLength(100)
						.HasColumnType("varchar");
            Property(p => p.tme_dataCriacao).IsRequired();
            Property(p => p.tme_dataAlteracao).IsRequired();
            Property(p => p.tme_situacao)
                        .IsRequired()
                        .HasColumnType("tinyint");
		}
	}
}
