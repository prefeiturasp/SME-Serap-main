using System;

namespace GestaoEscolar.Entities
{
    public class ACA_CurriculoPeriodo
    {
        public int cur_id { get; set; }
        public virtual ACA_Curriculo ACA_Curriculo { get; set; }
        public int crr_id { get; set; }
        public int crp_id { get; set; }
        public int crp_ordem { get; set; }
        public string crp_descricao { get; set; }
        public Byte crp_situacao { get; set; }
        public DateTime crp_dataCriacao { get; set; }
        public DateTime crp_dataAlteracao { get; set; }
        public virtual ACA_TipoCurriculoPeriodo ACA_TipoCurriculoPeriodo { get; set; }
        public int? tcp_id { get; set; }
    }
}
