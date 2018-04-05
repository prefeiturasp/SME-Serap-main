using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Entities.DTO
{
    public class UsersDTO
    {
        public Guid usu_id { get; set; }
        public string usu_login { get; set; }
        public Guid pes_id { get; set; }
        public string pes_nome { get; set; }
    }
}
