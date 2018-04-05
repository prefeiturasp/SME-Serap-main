using System;

namespace GestaoEscolar.Entities
{
    public class TUR_Turma
    {
        public long tur_id { get; set; }
        public virtual ESC_Escola ESC_Escola { get; set; }
        public int esc_id { get; set; }
        public string tur_codigo { get; set; }
        public string tur_descricao { get; set; }
        public virtual ACA_CalendarioAnual ACA_CalendarioAnual { get; set; }
        public int cal_id { get; set; }
        public virtual ACA_TipoTurno ACA_TipoTurno { get; set; }
        public int? ttn_id { get; set; }
        public Byte tur_situacao { get; set; }
        public DateTime tur_dataCriacao { get; set; }
        public DateTime tur_dataAlteracao { get; set; }
        public Byte tur_tipo { get; set; }
    }
}
