using ProvaSP.Model.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProvaSP.Web.ViewModel
{
    public class RelatorioAcompanhamentoEscola
    {
        public List<RelatorioItem> IndicadoresPessoa { get; set; }
        public List<RelatorioItem> IndicadoresTurma { get; set; }
        public IList<RelatorioAgrupamento> IndicadoresAgrupadosChave { get; set; }
    }
}