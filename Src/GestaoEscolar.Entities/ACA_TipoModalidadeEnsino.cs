using System;

namespace GestaoEscolar.Entities
{
    public class ACA_TipoModalidadeEnsino
    {
        public int tme_id { get; set; }
        public string tme_nome { get; set; }
        public Byte tme_situacao { get; set; }
        public DateTime tme_dataCriacao { get; set; }
        public DateTime tme_dataAlteracao { get; set; }
    }
}
