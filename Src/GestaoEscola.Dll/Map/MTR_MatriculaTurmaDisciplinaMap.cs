using GestaoEscolar.Entities;
using System.Data.Entity.ModelConfiguration;

namespace GestaoEscolar.Repository.Map
{
    public class MTR_MatriculaTurmaDisciplinaMap : EntityTypeConfiguration<MTR_MatriculaTurmaDisciplina>
	{
        public MTR_MatriculaTurmaDisciplinaMap()
		{
            ToTable("MTR_MatriculaTurmaDisciplina");

            HasKey(p => new { p.alu_id, p.mtu_id, p.mtd_id });

            Property(p => p.mtd_numeroChamada).IsOptional();
            Property(p => p.mtd_dataCriacao).IsRequired();
            Property(p => p.mtd_dataAlteracao).IsRequired();
            Property(p => p.mtd_situacao)
                        .IsRequired()
                        .HasColumnType("tinyint");

            HasRequired(p => p.TUR_TurmaDisciplina)
                .WithMany()
                .HasForeignKey(p => p.tud_id);

            HasRequired(p => p.MTR_MatriculaTurma)
                .WithMany()
                .HasForeignKey(p => new { p.alu_id, p.mtu_id });
		}
	}
}
