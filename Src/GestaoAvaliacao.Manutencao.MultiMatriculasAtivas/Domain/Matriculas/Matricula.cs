using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Manutencao.MultiMatriculasAtivas.Domain.Matriculas
{
    public class Matricula
    {
        public long AlunoId { get; set; }
        public int MtuId { get; set; }
        public Guid DreId { get; set; }
        public int EscolaId { get; set; }
        public string NomeDaEscola { get; set; }
        public string Turma { get; set; }
        public long TurmaId { get; set; }
        public DateTime DataDaMatricula { get; set; }
        public DateTime DataDeCriacao { get; set; }
        public DateTime DataDeAlteracao { get; set; }
        public int NumeroDeChamada { get; set; }
    }
}
