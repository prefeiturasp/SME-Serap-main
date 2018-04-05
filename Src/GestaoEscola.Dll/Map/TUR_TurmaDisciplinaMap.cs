using GestaoEscolar.Entities;
using System.Data.Entity.ModelConfiguration;

namespace GestaoEscolar.Repository.Map
{
    public class TUR_TurmaDisciplinaMap : EntityTypeConfiguration<TUR_TurmaDisciplina>
	{
        public TUR_TurmaDisciplinaMap()
		{
            ToTable("TUR_TurmaDisciplina");

            HasKey(p => p.tud_id);

            Property(p => p.tud_codigo)
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("varchar");
            Property(p => p.tud_nome)
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("varchar");
            Property(p => p.tud_dataCriacao).IsRequired();
            Property(p => p.tud_dataAlteracao).IsRequired();
            Property(p => p.tud_tipo)
                        .IsRequired()
                        .HasColumnType("tinyint");
            Property(p => p.tud_situacao)
                        .IsRequired()
                        .HasColumnType("tinyint");

            HasRequired(p => p.ACA_TipoDisciplina)
                .WithMany()
                .HasForeignKey(p => p.tds_id);

            HasRequired(p => p.TUR_Turma)
                .WithMany()
                .HasForeignKey(p => p.tur_id);
		}
	}
}
