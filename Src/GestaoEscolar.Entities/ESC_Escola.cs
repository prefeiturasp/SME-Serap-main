using System;

namespace GestaoEscolar.Entities
{
    public class ESC_Escola
    {
        public int esc_id { get; set; }
        public Guid ent_id { get; set; }
        public virtual SYS_UnidadeAdministrativa SYS_UnidadeAdministrativa { get; set; }
        public Guid uad_id { get; set; }
        public string esc_codigo { get; set; }
        public string esc_nome { get; set; }
        public Byte esc_situacao { get; set; }
        public DateTime esc_dataCriacao { get; set; }
        public DateTime esc_dataAlteracao { get; set; }
        public Guid? uad_idSuperiorGestao { get; set; }
    }
}
