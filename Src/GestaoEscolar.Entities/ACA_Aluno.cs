using System;

namespace GestaoEscolar.Entities
{
    public class ACA_Aluno
	{
		public long alu_id { get; set; }
		public string alu_nome { get; set; }
		public Guid ent_id { get; set; }
		public string alu_matricula { get; set; }
		public DateTime alu_dataCriacao { get; set; }
		public DateTime alu_dataAlteracao { get; set; }
		public Byte alu_situacao { get; set; }
		public MTR_MatriculaTurma MatriculaTurma { get; set; }
	}
}
