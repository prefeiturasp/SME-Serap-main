using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportarImagensRedacao
{
    public class PSP22Dto
    {
        [Name("Tipo_escola")]
        public string Tipo_escola { get; set; }
        [Name("DRE")]
        public string DRE { get; set; }
        [Name("ESC_EOL")]
        public string ESC_EOL { get; set; }
        [Name("Ano_Escolar")]
        public string Ano_Escolar { get; set; }
        [Name("Turma")]
        public string Turma { get; set; }
        [Name("Desc_Arquivo")]
        public string Desc_Arquivo { get; set; }
        [Name("Cod_EOL")]
        public string Cod_EOL { get; set; }
        [Name("Tipo_Prova")]
        public string Tipo_Prova { get; set; }
    }
}
