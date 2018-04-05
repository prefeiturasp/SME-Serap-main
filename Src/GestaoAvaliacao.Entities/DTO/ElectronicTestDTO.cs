using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Entities.DTO
{
    public class ElectronicTestDTO
    {
        public virtual long Id { get; set; }
        public string Description { get; set; }
        public int? NumberItem { get; set; }
        public int quantDiasRestantes { get; set; }
        public long alu_id { get; set; }
        public long tur_id { get; set; }
        public int FrequencyApplication { get; set; }
        public string FrequencyApplicationText { get; set; }
        public DateTime ApplicationEndDate { get; set; }
    }
}
