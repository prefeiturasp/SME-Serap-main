using System;

namespace GestaoEscolar.Entities
{
    public class ACA_Curriculo
    {
        public virtual ACA_Curso ACA_Curso { get; set; }
        public int cur_id { get; set; }
        public int crr_id { get; set; }
        public string crr_nome { get; set; }
        public Byte crr_situacao { get; set; }
        public DateTime crr_dataCriacao { get; set; }
        public DateTime crr_dataAlteracao { get; set; }
    }
}
