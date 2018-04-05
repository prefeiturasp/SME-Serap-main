using GestaoEscolar.Entities;
using System.Data.Entity.ModelConfiguration;

namespace GestaoEscolar.Repository.Map
{
    public class SYS_UnidadeAdministrativaMap : EntityTypeConfiguration<SYS_UnidadeAdministrativa>
	{
        public SYS_UnidadeAdministrativaMap()
		{
            ToTable("SYS_UnidadeAdministrativa");

            HasKey(p => new { p.ent_id, p.uad_id});

			Property(p => p.uad_nome)
						.IsRequired()
						.HasMaxLength(200)
						.HasColumnType("varchar");
            Property(p => p.uad_codigo)
                        .IsOptional()
                        .HasMaxLength(20)
                        .HasColumnType("varchar");
            Property(p => p.uad_sigla)
                        .IsOptional()
                        .HasMaxLength(50)
                        .HasColumnType("varchar");
            Property(p => p.uad_dataCriacao).IsRequired();
            Property(p => p.uad_dataAlteracao).IsRequired();
            Property(p => p.uad_situacao)
                        .IsRequired()
                        .HasColumnType("tinyint");
		}
	}
}
