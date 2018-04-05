using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProvaSP.Model.Entidades
{
    public class ResultadoItem
    {
        public string AnoEscolar { get; set; }
        public int NivelProficienciaID { get; set; }
        public int TotalAlunos { get; set; }
        public string ChavePai { get; set; }
        public string Chave { get; set; }
        public string Titulo { get; set; }
        public float Valor { get; set; }
        public float PercentualAbaixoDoBasico { get; set; }
        public float PercentualBasico { get; set; }
        public float PercentualAdequado { get; set; }
        public float PercentualAvancado { get; set; }
        public float PercentualSemProficiencia { get; set; }
    }
}
