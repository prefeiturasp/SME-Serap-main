using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Entities.DTO
{
    public class DresDTO
    {
        public Guid dre_id { get; set; }
        public string dre_nome { get; set; }
        public int esc_id { get; set; }
        public string esc_nome { get; set; }
    }
}
