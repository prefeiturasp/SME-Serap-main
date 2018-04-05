using System;

namespace GestaoEscolar.Entities
{
    public class ACA_CalendarioAnual
    {
        public int cal_id { get; set; }
        public Guid ent_id { get; set; }
        public bool cal_padrao { get; set; }
        public int cal_ano { get; set; }
        public string cal_descricao { get; set; }
        public DateTime cal_dataInicio { get; set; }
        public DateTime cal_dataFim { get; set; }
        public Byte cal_situacao { get; set; }
        public DateTime cal_dataCriacao { get; set; }
        public DateTime cal_dataAlteracao { get; set; }
    }
}
