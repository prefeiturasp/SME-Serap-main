using GestaoEscolar.Entities;
using System.Data.Entity.ModelConfiguration;

namespace GestaoEscolar.Repository.Map
{
    public class ESC_EscolaMap : EntityTypeConfiguration<ESC_Escola>
	{
        public ESC_EscolaMap()
		{
            ToTable("ESC_Escola");

            HasKey(p => p.esc_id);

            Property(p => p.esc_nome)
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("varchar");
            Property(p => p.esc_codigo)
                        .IsOptional()
                        .HasMaxLength(20)
                        .HasColumnType("varchar");
            Property(p => p.esc_dataCriacao).IsRequired();
            Property(p => p.esc_dataAlteracao).IsRequired();
            Property(p => p.esc_situacao)
                        .IsRequired()
                        .HasColumnType("tinyint");

            HasOptional(p => p.SYS_UnidadeAdministrativa)
                .WithMany()
                .HasForeignKey(p => new { p.ent_id, p.uad_idSuperiorGestao });
		}
	}
}
