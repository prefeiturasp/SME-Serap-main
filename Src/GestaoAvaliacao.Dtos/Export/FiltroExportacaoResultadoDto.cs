using System;

namespace GestaoAvaliacao.Dtos
{
    public class FiltroExportacaoResultadoDto
    {
        public DateTime? DataInicio { get; set; }
        public DateTime? DataFim { get; set; }
        public long? ProvaSerapId { get; set; }
        public int QuantidadeRegistros { get; set; }
        public int NumeroPagina { get; set; }
    }
}
