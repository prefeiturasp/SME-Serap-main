using GestaoEscolar.Entities;
using System.Data.Entity.ModelConfiguration;

namespace GestaoEscolar.Repository.Map
{
    public class MTR_MatriculaTurmaMap : EntityTypeConfiguration<MTR_MatriculaTurma>
	{
        public MTR_MatriculaTurmaMap()
		{
            ToTable("MTR_MatriculaTurma");

            HasKey(p => new { p.alu_id, p.mtu_id });

            Property(p => p.mtu_numeroChamada).IsOptional();
            Property(p => p.mtu_dataCriacao).IsRequired();
            Property(p => p.mtu_dataAlteracao).IsRequired();
            Property(p => p.mtu_situacao)
                        .IsRequired()
                        .HasColumnType("tinyint");

            HasRequired(p => p.ESC_Escola)
                .WithMany()
                .HasForeignKey(p => p.esc_id);

            HasRequired(p => p.ACA_Aluno)
                .WithMany()
                .HasForeignKey(p => p.alu_id);

            HasRequired(p => p.TUR_TurmaCurriculo)
                .WithMany()
                .HasForeignKey(p => new { p.tur_id, p.cur_id, p.crr_id, p.crp_id });
		}
	}
}
