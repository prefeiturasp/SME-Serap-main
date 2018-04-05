using GestaoEscolar.Entities;
using System.Data.Entity.ModelConfiguration;

namespace GestaoEscolar.Repository.Map
{
    public class TUR_TurmaMap : EntityTypeConfiguration<TUR_Turma>
	{
        public TUR_TurmaMap()
		{
            ToTable("TUR_Turma");

            HasKey(p => p.tur_id);

            Property(p => p.tur_codigo)
                        .IsOptional()
                        .HasMaxLength(30)
                        .HasColumnType("varchar");
            Property(p => p.tur_descricao)
                        .IsOptional()
                        .HasMaxLength(2000)
                        .HasColumnType("varchar");
            Property(p => p.tur_dataCriacao).IsRequired();
            Property(p => p.tur_dataAlteracao).IsRequired();
            Property(p => p.tur_situacao)
                        .IsRequired()
                        .HasColumnType("tinyint");
            Property(p => p.tur_tipo)
                        .IsRequired()
                        .HasColumnType("tinyint");

            HasRequired(p => p.ESC_Escola)
                .WithMany()
                .HasForeignKey(p => p.esc_id);

            HasRequired(p => p.ACA_CalendarioAnual)
                .WithMany()
                .HasForeignKey(p => p.cal_id);

            HasOptional(p => p.ACA_TipoTurno)
                .WithMany()
                .HasForeignKey(p => p.ttn_id);
		}
	}
}
