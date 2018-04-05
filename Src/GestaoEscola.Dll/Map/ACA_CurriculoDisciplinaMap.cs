using GestaoEscolar.Entities;
using System.Data.Entity.ModelConfiguration;

namespace GestaoEscolar.Repository.Map
{
    public class ACA_CurriculoDisciplinaMap : EntityTypeConfiguration<ACA_CurriculoDisciplina>
	{
        public ACA_CurriculoDisciplinaMap()
		{
            ToTable("ACA_CurriculoDisciplina");

            HasKey(p => new { p.cur_id, p.crr_id, p.crp_id, p.tds_id });

            Property(p => p.crd_dataCriacao).IsRequired();
            Property(p => p.crd_dataAlteracao).IsRequired();
            Property(p => p.crd_situacao)
                        .IsRequired()
                        .HasColumnType("tinyint");
            Property(p => p.crd_tipo)
                        .IsRequired()
                        .HasColumnType("tinyint");

            HasRequired(p => p.ACA_TipoDisciplina)
                .WithMany()
                .HasForeignKey(p => p.tds_id);

            HasRequired(p => p.ACA_CurriculoPeriodo)
                .WithMany()
                .HasForeignKey(p => new { p.cur_id, p.crr_id, p.crp_id });
		}
	}
}
