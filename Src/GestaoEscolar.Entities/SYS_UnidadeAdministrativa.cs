using System;

namespace GestaoEscolar.Entities
{
    public class SYS_UnidadeAdministrativa
    {
        public Guid ent_id { get; set; }
        public Guid uad_id { get; set; }
        public string uad_codigo { get; set; }
        public string uad_nome { get; set; }
        public string uad_sigla { get; set; }
        public Byte uad_situacao { get; set; }
        public DateTime uad_dataCriacao { get; set; }
        public DateTime uad_dataAlteracao { get; set; }
    }
}
