using System;

namespace GestaoEscolar.Entities
{
    public class ACA_CurriculoDisciplina
    {
        public int cur_id { get; set; }
        public int crr_id { get; set; }
        public virtual ACA_CurriculoPeriodo ACA_CurriculoPeriodo { get; set; }
        public int crp_id { get; set; }
        public virtual ACA_TipoDisciplina ACA_TipoDisciplina { get; set; }
        public int tds_id { get; set; }
        public Byte crd_tipo { get; set; }
        public Byte crd_situacao { get; set; }
        public DateTime crd_dataCriacao { get; set; }
        public DateTime crd_dataAlteracao { get; set; }
    }
}
