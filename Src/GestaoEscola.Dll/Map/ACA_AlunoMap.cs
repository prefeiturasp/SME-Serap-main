using GestaoEscolar.Entities;
using System.Data.Entity.ModelConfiguration;

namespace GestaoEscolar.Repository.Map
{
    public class ACA_AlunoMap : EntityTypeConfiguration<ACA_Aluno>
	{
        public ACA_AlunoMap()
		{
            ToTable("ACA_Aluno");

            HasKey(p => p.alu_id);

			Property(p => p.alu_nome)
						.IsRequired()
						.HasMaxLength(200)
						.HasColumnType("varchar");
            Property(p => p.alu_matricula)
                        .IsOptional()
                        .HasMaxLength(50)
                        .HasColumnType("varchar");
            Property(p => p.alu_dataCriacao).IsRequired();
            Property(p => p.alu_dataAlteracao).IsRequired();
            Property(p => p.alu_situacao)
                        .IsRequired()
                        .HasColumnType("tinyint");
            Property(p => p.ent_id).IsRequired();
		}
	}
}
