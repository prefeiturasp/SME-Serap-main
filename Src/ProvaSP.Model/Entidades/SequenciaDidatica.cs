using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProvaSP.Model.Entidades
{
    public class SequenciaDidatica
    {
        public string Edicao { get; set; }
        public int AnoEscolar { get; set; }
        public int AreaConhecimentoId { get; set; }
        public int CorteId { get; set; }
        public string Titulo { get; set; }
        public string Texto { get; set; }
        public string Link { get; set; }
        public string CorteNome { get; set; }
    }
}
