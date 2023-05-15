using System.Collections.Generic;

namespace GestaoAvaliacao.Entities.DTO
{
    public class CsvBlockImportDTO
    {
        public CsvBlockImportDTO()
        {
            Erros = new List<ErrorCsvBlockImportDTO>();
        }

        public int QtdeSucesso { get; set; }
        public int QtdeErros { get; set; }
        public List<ErrorCsvBlockImportDTO> Erros { get; set; }
    }
}
