using System;

namespace GestaoEscolar.Entities
{
    public class ACA_TipoTurno
    {
        public int ttn_id { get; set; }
        public string ttn_nome { get; set; }
        public Byte ttn_situacao { get; set; }
        public DateTime ttn_dataCriacao { get; set; }
        public DateTime ttn_dataAlteracao { get; set; }
    }
}
