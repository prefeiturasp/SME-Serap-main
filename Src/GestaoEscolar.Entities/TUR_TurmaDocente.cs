using System;

namespace GestaoEscolar.Entities
{
    public class TUR_TurmaDocente
    {
        public virtual TUR_TurmaDisciplina TUR_TurmaDisciplina { get; set; }
        public long tud_id { get; set; }
        public int tdt_id { get; set; }
        public virtual ACA_Docente ACA_Docente { get; set; }
        public long doc_id { get; set; }
        public Byte tdt_situacao { get; set; }
        public DateTime tdt_dataCriacao { get; set; }
        public DateTime tdt_dataAlteracao { get; set; }
        public Byte tdt_posicao { get; set; }
    }
}
