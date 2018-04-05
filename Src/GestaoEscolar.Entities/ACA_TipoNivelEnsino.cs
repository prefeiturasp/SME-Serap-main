using System;

namespace GestaoEscolar.Entities
{
    public class ACA_TipoNivelEnsino
    {
        public int tne_id { get; set; }
        public string tne_nome { get; set; }
        public Byte tne_situacao { get; set; }
        public DateTime tne_dataCriacao { get; set; }
        public DateTime tne_dataAlteracao { get; set; }
        public int tne_ordem { get; set; }
    }
}
