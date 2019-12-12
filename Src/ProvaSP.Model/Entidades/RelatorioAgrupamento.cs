using System;
using System.Collections.Generic;
using System.Linq;

namespace ProvaSP.Model.Entidades
{
    public class RelatorioAgrupamento
    {
        public RelatorioAgrupamento()
        {
            IndicadoresEscola = new List<RelatorioItem>();
            GridIndicadoresEscola = new List<RelatorioItemAgrupado>();
        }

        public string Chave { get; set; }
        public string Titulo { get; set; }
        public IList<RelatorioItem> IndicadoresEscola { get; set; }
        public IList<RelatorioItemAgrupado> GridIndicadoresEscola { get; set; }
    }
}