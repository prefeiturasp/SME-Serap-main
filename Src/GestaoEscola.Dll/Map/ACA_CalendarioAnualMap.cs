using GestaoEscolar.Entities;
using System.Data.Entity.ModelConfiguration;

namespace GestaoEscolar.Repository.Map
{
    public class ACA_CalendarioAnualMap : EntityTypeConfiguration<ACA_CalendarioAnual>
	{
        public ACA_CalendarioAnualMap()
		{
            ToTable("ACA_CalendarioAnual");

            HasKey(p => p.cal_id);

			Property(p => p.cal_descricao)
						.IsRequired()
						.HasMaxLength(200)
						.HasColumnType("varchar");
            Property(p => p.cal_dataInicio)
                        .IsRequired()
                        .HasColumnType("Date");
            Property(p => p.cal_dataFim)
                        .IsRequired()
                        .HasColumnType("Date");
            Property(p => p.cal_padrao).IsRequired();
            Property(p => p.cal_ano).IsRequired();
            Property(p => p.cal_dataCriacao).IsRequired();
            Property(p => p.cal_dataAlteracao).IsRequired();
            Property(p => p.cal_situacao)
                        .IsRequired()
                        .HasColumnType("tinyint");
            Property(p => p.ent_id).IsRequired();
		}
	}
}
