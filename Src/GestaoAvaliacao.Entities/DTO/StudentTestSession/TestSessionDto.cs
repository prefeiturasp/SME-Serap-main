using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Entities.DTO
{
    public class TestSessionDto
    {
        public string DataEHoraInicial { get; set; }
        public string DataEHoraFinal { get; set; }
        public string TempoTotalFormatado { get; set; }
        public TimeSpan TempoTotal { get; set; }
    }
}
