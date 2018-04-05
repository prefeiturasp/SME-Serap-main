using GestaoEscolar.Entities;
using System.Data.Entity.ModelConfiguration;

namespace GestaoEscolar.Repository.Map
{
    public class ACA_TipoCurriculoPeriodoMap : EntityTypeConfiguration<ACA_TipoCurriculoPeriodo>
	{
		public ACA_TipoCurriculoPeriodoMap()
		{
			ToTable("ACA_TipoCurriculoPeriodo");

			HasKey(p => p.tcp_id);

			Property(p => p.tcp_descricao)
						.IsRequired()
						.HasMaxLength(100)
						.HasColumnType("varchar");
			Property(p => p.tcp_dataCriacao).IsRequired();
			Property(p => p.tcp_dataAlteracao).IsRequired();
			Property(p => p.tcp_ordem)
						.IsRequired()
						.HasColumnType("tinyint");
			Property(p => p.tcp_situacao)
						.IsRequired()
						.HasColumnType("tinyint");

			HasRequired(p => p.ACA_TipoNivelEnsino)
				.WithMany()
				.HasForeignKey(p => p.tne_id);

			HasRequired(p => p.ACA_TipoModalidadeEnsino)
				.WithMany()
				.HasForeignKey(p => p.tme_id);
		}
	}
}
