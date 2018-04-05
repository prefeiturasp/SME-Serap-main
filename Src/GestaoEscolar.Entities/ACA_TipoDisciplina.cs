using System;

namespace GestaoEscolar.Entities
{
    public class ACA_TipoDisciplina
    {
        public int tds_id { get; set; }
        public virtual ACA_TipoNivelEnsino ACA_TipoNivelEnsino { get; set; }
        public int tne_id { get; set; }
        public string tds_nome { get; set; }
        public Byte tds_situacao { get; set; }
        public DateTime tds_dataCriacao { get; set; }
        public DateTime tds_dataAlteracao { get; set; }
    }
}
