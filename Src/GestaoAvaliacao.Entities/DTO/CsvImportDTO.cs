using System.Collections.Generic;

namespace GestaoAvaliacao.Entities.DTO
{
    public class CsvImportDTO
    {
        public CsvImportDTO()
        {
            Erros = new List<ErrosImportacaoCSV>();
        }

        public int QtdeSucesso { get; set; }
        public int QtdeErros { get; set; }
        public List<ErrosImportacaoCSV> Erros { get; set; }
    }
}
