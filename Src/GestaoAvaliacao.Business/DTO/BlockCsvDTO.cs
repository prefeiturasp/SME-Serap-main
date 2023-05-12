using CsvHelper.Configuration.Attributes;

namespace GestaoAvaliacao.Business.DTO
{
    internal class BlockCsvDTO
    {
        [Name("NumeroBloco")]
        public string NumeroBloco { get; set; }
        [Name("CodigoItem")]
        public string CodigoItem { get; set; }
    }
}
