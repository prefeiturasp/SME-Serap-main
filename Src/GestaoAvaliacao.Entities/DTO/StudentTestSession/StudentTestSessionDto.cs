using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Entities.DTO
{
    public class StudentTestSessionDto
    {
        public string NumeroDaChamada { get; set; }
        public string NomeDoAluno { get; set; }
        public string TempoTotalDaSessao { get; set; }
        public List<TestSessionDto> Session { get; set; }
    }
}
