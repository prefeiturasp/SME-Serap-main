using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Dtos.StudentTestAccoplishments
{
    public class StudentTestTimeDto
    {
        public long TurId { get; set; }
        public long TestId { get; set; }
        public string TempoDeDuracao { get; set; }
        public int TempoDeDuracaoEmSegundos { get; set; }
        public string DataDeFinalizacaoDaProva { get; set; }
    }
}
