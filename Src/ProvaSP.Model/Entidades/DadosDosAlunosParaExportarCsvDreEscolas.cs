using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProvaSP.Model.Entidades
{
    public class DadosDosAlunosParaExportarCsvDreEscolas
    {
        public string NomeDaEscola { get; set; }
        public int Edicao { get; set; }
        public int NivelProficienciaID { get; set; }
        public string Nome { get; set; }
        public string Matricula { get; set; }
        public int AnoEscolar { get; set; }
        public string CodigoDaTurmaDoAluno { get; set; }
        public string Periodo { get; set; }
        public decimal Media { get; set; }
        public string NivelProficiencia { get; set; }
    }
}
