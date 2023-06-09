using CsvHelper.Configuration.Attributes;

namespace GestaoAvaliacao.Business.DTO
{
    internal class CadernoCsvDTO
    {
        [Name("NumeroCaderno")]
        public string NumeroCaderno { get; set; }

        [Name("NumeroBloco")]
        public string NumeroBloco { get; set; }
    }
}
