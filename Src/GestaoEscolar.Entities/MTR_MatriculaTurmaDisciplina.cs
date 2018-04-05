using System;

namespace GestaoEscolar.Entities
{
    public class MTR_MatriculaTurmaDisciplina
    {
        public long alu_id { get; set; }
        public virtual MTR_MatriculaTurma MTR_MatriculaTurma { get; set; }
        public int mtu_id { get; set; }
        public int mtd_id { get; set; }
        public virtual TUR_TurmaDisciplina TUR_TurmaDisciplina { get; set; }
        public long tud_id { get; set; }
        public int mtd_numeroChamada { get; set; }
        public Byte mtd_situacao { get; set; }
        public DateTime mtd_dataCriacao { get; set; }
        public DateTime mtd_dataAlteracao { get; set; }
    }
}
