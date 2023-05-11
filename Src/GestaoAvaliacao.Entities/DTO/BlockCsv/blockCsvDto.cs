using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Entities.DTO.BlockCsv
{
    public class blockCsvDto
    {
        [Name("NumeroBloco")]
        public string NumeroBloco { get; set; }
        [Name("CodigoItem")]
        public string CodigoItem { get; set; }
    }
}