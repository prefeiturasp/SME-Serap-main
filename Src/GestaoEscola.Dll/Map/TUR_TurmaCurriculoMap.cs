using GestaoEscolar.Entities;
using System.Data.Entity.ModelConfiguration;

namespace GestaoEscolar.Repository.Map
{
    public class TUR_TurmaCurriculoMap : EntityTypeConfiguration<TUR_TurmaCurriculo>
	{
        public TUR_TurmaCurriculoMap()
		{
            ToTable("TUR_TurmaCurriculo");

            HasKey(p => new { p.tur_id, p.cur_id, p.crr_id, p.crp_id });

            Property(p => p.tcr_dataCriacao).IsRequired();
            Property(p => p.tcr_dataAlteracao).IsRequired();
            Property(p => p.tcr_situacao)
                        .IsRequired()
                        .HasColumnType("tinyint");

            HasRequired(p => p.TUR_Turma)
                .WithMany()
                .HasForeignKey(p => p.tur_id);

            HasRequired(p => p.ACA_CurriculoPeriodo)
                .WithMany()
                .HasForeignKey(p => new { p.cur_id, p.crr_id, p.crp_id });
		}
	}
}
