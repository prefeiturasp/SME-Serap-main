using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProvaSP.Model.Entidades
{
    public class HabilidadeTema
    {
        public HabilidadeTema()
        {
            Itens = new List<HabilidadeItem>();
        }
        public string Titulo { get; set; }
        public List<HabilidadeItem> Itens { get; set; }
    }
}
