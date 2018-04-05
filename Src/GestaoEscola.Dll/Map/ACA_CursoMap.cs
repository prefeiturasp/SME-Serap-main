using GestaoEscolar.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;

namespace GestaoEscolar.Repository.Map
{
    public class ACA_CursoMap : EntityTypeConfiguration<ACA_Curso>
	{
        public ACA_CursoMap()
		{
            ToTable("ACA_Curso");

            HasKey(p => p.cur_id);

			Property(p => p.cur_codigo)
						.IsOptional()
						.HasMaxLength(10)
						.HasColumnType("varchar");
            Property(p => p.cur_nome)
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("varchar");
            Property(p => p.cur_nome_abreviado)
                        .IsOptional()
                        .HasMaxLength(20)
                        .HasColumnType("varchar");
            Property(p => p.cur_dataCriacao).IsRequired();
            Property(p => p.cur_dataAlteracao).IsRequired();
            Property(p => p.cur_situacao)
                        .IsRequired()
                        .HasColumnType("tinyint");
            Property(p => p.ent_id).IsRequired();

            HasRequired(p => p.ACA_TipoNivelEnsino)
                .WithMany()
                .HasForeignKey(p => p.tne_id);

            HasRequired(p => p.ACA_TipoModalidadeEnsino)
                .WithMany()
                .HasForeignKey(p => p.tme_id);

            Property(p => p.ent_id)
                .HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("IX_ACA_Curso_ent_id")));
		}
	}
}
