using GestaoEscolar.Entities;
using System.Data.Entity.ModelConfiguration;

namespace GestaoEscolar.Repository.Map
{
    public class ACA_TipoDisciplinaMap : EntityTypeConfiguration<ACA_TipoDisciplina>
	{
        public ACA_TipoDisciplinaMap()
		{
            ToTable("ACA_TipoDisciplina");

            HasKey(p => p.tds_id);

			Property(p => p.tds_nome)
						.IsRequired()
						.HasMaxLength(100)
						.HasColumnType("varchar");
            Property(p => p.tds_dataCriacao).IsRequired();
            Property(p => p.tds_dataAlteracao).IsRequired();
            Property(p => p.tds_situacao)
                        .IsRequired()
                        .HasColumnType("tinyint");

            HasRequired(p => p.ACA_TipoNivelEnsino)
                .WithMany()
                .HasForeignKey(p => p.tne_id);
		}
	}
}
