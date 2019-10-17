using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProvaSP.Model.Entidades
{
    public class Resultado
    {
        public string AnoEscolar { get; set; }
        public string CicloId { get; set; }
        public List<ResultadoItem> Agregacao { get; set; }
        public List<ResultadoItem> Itens { get; set; }
        public List<HabilidadeTema> Habilidades { get; set; }
        public List<Proficiencia> Proficiencias { get; set; }
    }
}
