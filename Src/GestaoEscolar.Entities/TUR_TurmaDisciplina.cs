using System;

namespace GestaoEscolar.Entities
{
    public class TUR_TurmaDisciplina
    {
        public long tud_id { get; set; }

        public virtual TUR_Turma TUR_Turma { get; set; }
        public long tur_id { get; set; }
        public virtual ACA_TipoDisciplina ACA_TipoDisciplina { get; set; }
        public int tds_id { get; set; }
        public string tud_codigo { get; set; }
        public string tud_nome { get; set; }
        public Byte tud_tipo { get; set; }
        public Byte tud_situacao { get; set; }
        public DateTime tud_dataCriacao { get; set; }
        public DateTime tud_dataAlteracao { get; set; }
    }
}
