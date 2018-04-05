using GestaoEscolar.Entities;
using System.Data.Entity.ModelConfiguration;

namespace GestaoEscolar.Repository.Map
{
    public class ACA_DocenteMap : EntityTypeConfiguration<ACA_Docente>
	{
		public ACA_DocenteMap()
		{
			ToTable("ACA_Docente");

			HasKey(p => p.doc_id);

			Property(p => p.doc_nome)
						.IsRequired()
						.HasMaxLength(200)
						.HasColumnType("varchar");
			Property(p => p.doc_dataCriacao).IsRequired();
			Property(p => p.doc_dataAlteracao).IsRequired();
			Property(p => p.doc_situacao)
						.IsRequired()
						.HasColumnType("tinyint");
		}
	}
}
