using GestaoEscolar.Entities;
using System.Data.Entity.ModelConfiguration;

namespace GestaoEscolar.Repository.Map
{
    public class ACA_CurriculoMap : EntityTypeConfiguration<ACA_Curriculo>
	{
        public ACA_CurriculoMap()
		{
            ToTable("ACA_Curriculo");

            HasKey(p => new { p.cur_id, p.crr_id });

			Property(p => p.crr_nome)
						.IsOptional()
						.HasMaxLength(200)
						.HasColumnType("varchar");
            Property(p => p.crr_dataCriacao).IsRequired();
            Property(p => p.crr_dataAlteracao).IsRequired();
            Property(p => p.crr_situacao)
                        .IsRequired()
                        .HasColumnType("tinyint");

            HasRequired(p => p.ACA_Curso)
                .WithMany()
                .HasForeignKey(p => p.cur_id);
		}
	}
}
