using CsvHelper.Configuration.Attributes;

namespace GestaoAvaliacao.Business.DTO
{

    internal class ReportStudiesCsvDto
    {
        [Name("cod")]
        public long Codigo { get; set; }
        [Name("grupo")]
        public string TipoGrupo { get; set; }
        [Name("destinatario")]
        public string Destinatario { get; set; }
    }
}

