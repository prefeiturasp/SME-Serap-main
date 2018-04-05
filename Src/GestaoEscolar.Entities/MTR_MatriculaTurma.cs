using System;

namespace GestaoEscolar.Entities
{
    public class MTR_MatriculaTurma
    {
        public virtual ACA_Aluno ACA_Aluno { get; set; }
        public long alu_id { get; set; }
        public int mtu_id { get; set; }
        public virtual ESC_Escola ESC_Escola { get; set; }
        public int esc_id { get; set; }
        public virtual TUR_TurmaCurriculo TUR_TurmaCurriculo { get; set; }
        public long tur_id { get; set; }
        public int cur_id { get; set; }
        public int crr_id { get; set; }
        public int crp_id { get; set; }
        public Byte mtu_situacao { get; set; }
        public DateTime mtu_dataCriacao { get; set; }
        public DateTime mtu_dataAlteracao { get; set; }
        public int mtu_numeroChamada { get; set; }
    }
}
