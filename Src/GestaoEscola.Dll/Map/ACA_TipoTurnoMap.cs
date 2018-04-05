using GestaoEscolar.Entities;
using System.Data.Entity.ModelConfiguration;

namespace GestaoEscolar.Repository.Map
{
    public class ACA_TipoTurnoMap : EntityTypeConfiguration<ACA_TipoTurno>
	{
        public ACA_TipoTurnoMap()
		{
            ToTable("ACA_TipoTurno");

            HasKey(p => p.ttn_id);

			Property(p => p.ttn_nome)
						.IsRequired()
						.HasMaxLength(100)
						.HasColumnType("varchar");
            Property(p => p.ttn_dataCriacao).IsRequired();
            Property(p => p.ttn_dataAlteracao).IsRequired();
            Property(p => p.ttn_situacao)
                        .IsRequired()
                        .HasColumnType("tinyint");
		}
	}
}
