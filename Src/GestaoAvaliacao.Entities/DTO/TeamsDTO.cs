using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Entities.DTO
{
    public class TeamsDTO
    {
        public long test_id { get; set; }
        public long tur_id { get; set; }
        public string tur_codigo { get; set; }
        public int? esc_id { get; set; }
    }
}
