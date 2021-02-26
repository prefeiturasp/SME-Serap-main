using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Dtos.StudentTestAccoplishments
{
    public class StudentTestTimeResultDto
    {
        public StudentTestTimeResultDto()
        {
            ListaProvasDoAnoCorrente = new List<StudentTestTimeListaDto>();
            ListaProvasDosAnosAnteriores = new List<StudentTestTimeListaDto>();
        }
        public int Ano { get; set; }
        public bool ProvasDoANoCorrente { get; set; }
        public List<StudentTestTimeListaDto> ListaProvasDoAnoCorrente { get; set; }
        public List<StudentTestTimeListaDto> ListaProvasDosAnosAnteriores { get; set; }
    }
}
