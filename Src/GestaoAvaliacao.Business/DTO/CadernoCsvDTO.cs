using CsvHelper.Configuration.Attributes;

namespace GestaoAvaliacao.Business.DTO
{
    internal class CadernoCsvDTO
    {
        [Name("Numero do caderno")]
        public string NumeroCaderno { get; set; }

        [Name("Numero do bloco")]
        public string NumeroBloco { get; set; }
    }
}
