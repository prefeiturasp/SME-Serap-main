using System;

namespace GestaoEscolar.Entities
{
    public class ACA_Docente
    {
        public long doc_id { get; set; }
        public string doc_nome { get; set; }
        public Byte doc_situacao { get; set; }
        public DateTime doc_dataCriacao { get; set; }
        public DateTime doc_dataAlteracao { get; set; }
		public Guid ent_id { get; set; }
		public Guid pes_id { get; set; }
    }
}
