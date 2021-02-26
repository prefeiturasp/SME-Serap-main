using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Dtos.StudentTestAccoplishments
{
    public class StudentTestTimeListaDto
    {
        public string NomeDaProva { get; set; }
        public string Periodo { get; set; }
        public string DataDeFinalizacao { get; set; }
        public string TempoDeProva {get; set; }
        public int QuantidadeDeItens { get; set; }
    }
}
