using CsvHelper.Configuration.Attributes;

namespace GestaoAvaliacao.Business.DTO
{

    internal class ReportStudiesCsvDto
    {
        [Name("Codigo")]
        public long Codigo { get; set; }
        [Name("TipoGrupo")]
        public string TipoGrupo { get; set; }
        [Name("Destinatario")]
        public string Destinatario { get; set; }
    }
}

