using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoAvaliacao.MongoEntities.DTO
{
    public class ResponseChangeLogDTO
    {
        public Guid Ent_id { get; set; }
        public Guid Usu_id { get; set; }
        public string usu_login { get; set; }
        public Guid Dre_id { get; set; }
        public string dre_nome { get; set; }
        public int Esc_id { get; set; }
        public string esc_nome { get; set; }
        public long Tur_id { get; set; }
        public string tur_nome { get; set; }
        public long Test_id { get; set; }
        public long Alu_id { get; set; }
        public string alu_nome { get; set; }
        public long Item_Id { get; set; }
        public string OrderItem { get; set; }
        public string valorAnterior { get; set; }
        public string valorAtual { get; set; }
        public bool Automatic { get; set; }
        public DateTime dataCriacao { get; set; }
    }

}
