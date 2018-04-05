using System;

namespace GestaoEscolar.Entities
{
    public class TUR_TurmaCurriculo
    {
        public virtual TUR_Turma TUR_Turma { get; set; }
        public long tur_id { get; set; }
        public int cur_id { get; set; }
        public int crr_id { get; set; }
        public virtual ACA_CurriculoPeriodo ACA_CurriculoPeriodo { get; set; }
        public int crp_id { get; set; }
        public Byte tcr_situacao { get; set; }
        public DateTime tcr_dataCriacao { get; set; }
        public DateTime tcr_dataAlteracao { get; set; }
    }
}
