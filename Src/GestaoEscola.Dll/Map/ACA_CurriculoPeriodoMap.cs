using GestaoEscolar.Entities;
using System.Data.Entity.ModelConfiguration;

namespace GestaoEscolar.Repository.Map
{
    public class ACA_CurriculoPeriodoMap : EntityTypeConfiguration<ACA_CurriculoPeriodo>
	{
        public ACA_CurriculoPeriodoMap()
		{
            ToTable("ACA_CurriculoPeriodo");

            HasKey(p => new { p.cur_id, p.crr_id, p.crp_id });

            Property(p => p.crp_descricao)
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("varchar");
			Property(p => p.crp_ordem).IsRequired();
            Property(p => p.crp_dataCriacao).IsRequired();
            Property(p => p.crp_dataAlteracao).IsRequired();
            Property(p => p.crp_situacao)
                        .IsRequired()
                        .HasColumnType("tinyint");

            HasRequired(p => p.ACA_Curriculo)
                .WithMany()
                .HasForeignKey(p => new { p.cur_id, p.crr_id });

            HasOptional(p => p.ACA_TipoCurriculoPeriodo)
                .WithMany()
                .HasForeignKey(p => p.tcp_id);
		}
	}
}
