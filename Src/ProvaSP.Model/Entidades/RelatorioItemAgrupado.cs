using System;
using System.Collections.Generic;
using System.Linq;

namespace ProvaSP.Model.Entidades
{
    public class RelatorioItemAgrupado
    {
        public int TipoRespondente { get; set; }
        public string DescricaoRespondente { get; set; }
        public int TotalEsperado { get; set; }
        public int QuantidadePreenchido { get; set; }
        public int PercentualPreenchido { get; set; }
    }
}
