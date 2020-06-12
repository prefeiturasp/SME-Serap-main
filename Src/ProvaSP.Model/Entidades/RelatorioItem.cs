using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProvaSP.Model.Entidades
{
    public class RelatorioItem
    {
        public string Chave { get; set; }
        public string Titulo { get; set; }
        public string Atributo { get; set; }
        public int AtributoID { get; set; }
        public string Valor { get; set; }
    }
}
