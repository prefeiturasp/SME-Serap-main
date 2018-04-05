using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProvaSP.Model.Entidades
{
    public class HabilidadeItem
    {
        public string OrigemTitulo { get; set; }
        public string Codigo { get; set; }
        public string Descricao { get; set; }
        public float PercentualAcertosNivelSME { get; set; }
        public float PercentualAcertosNivelDRE { get; set; }
        public float PercentualAcertosNivelEscola { get; set; }
        public float PercentualAcertosNivelTurma { get; set; }
    }
}
