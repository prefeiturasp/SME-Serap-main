using System;

namespace GestaoEscolar.Entities
{
    public class ACA_TipoCurriculoPeriodo
    {
        public int tcp_id { get; set; }
        public virtual ACA_TipoNivelEnsino ACA_TipoNivelEnsino { get; set; }
        public int tne_id { get; set; }
        public virtual ACA_TipoModalidadeEnsino ACA_TipoModalidadeEnsino { get; set; }
        public int tme_id { get; set; }
        public string tcp_descricao { get; set; }
        public Byte tcp_ordem { get; set; }
        public Byte tcp_situacao { get; set; }
        public DateTime tcp_dataCriacao { get; set; }
        public DateTime tcp_dataAlteracao { get; set; }
    }
}
