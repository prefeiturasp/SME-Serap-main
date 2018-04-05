using GestaoEscolar.Entities;
using System.Data.Entity.ModelConfiguration;

namespace GestaoEscolar.Repository.Map
{
    public class TUR_TurmaDocenteMap : EntityTypeConfiguration<TUR_TurmaDocente>
	{
        public TUR_TurmaDocenteMap()
		{
            ToTable("TUR_TurmaDocente");

            HasKey(p => new { p.tud_id, p.tdt_id });

            Property(p => p.tdt_dataCriacao).IsRequired();
            Property(p => p.tdt_dataAlteracao).IsRequired();
            Property(p => p.tdt_situacao)
                        .IsRequired()
                        .HasColumnType("tinyint");
            Property(p => p.tdt_posicao)
                        .IsRequired()
                        .HasColumnType("tinyint");

            HasRequired(p => p.ACA_Docente)
                .WithMany()
                .HasForeignKey(p => p.doc_id);

            HasRequired(p => p.TUR_TurmaDisciplina)
                .WithMany()
                .HasForeignKey(p => p.tud_id);
		}
	}
}
