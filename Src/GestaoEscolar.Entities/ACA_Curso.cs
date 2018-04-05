using System;

namespace GestaoEscolar.Entities
{
    public class ACA_Curso
    {
        public int cur_id { get; set; }
        public Guid ent_id { get; set; }
        public virtual ACA_TipoNivelEnsino ACA_TipoNivelEnsino { get; set; }
        public int tne_id { get; set; }
        public virtual ACA_TipoModalidadeEnsino ACA_TipoModalidadeEnsino { get; set; }
        public int tme_id { get; set; }
        public string cur_codigo { get; set; }
        public string cur_nome { get; set; }
        public string cur_nome_abreviado { get; set; }
        public Byte cur_situacao { get; set; }
        public DateTime cur_dataCriacao { get; set; }
        public DateTime cur_dataAlteracao { get; set; }
    }
}
