using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportarImagensRedacao
{
    public class PSPRC2022
    {

        [Name("tipo_esc")]
        public string Tipo_escola { get; set; }
        [Name("DRE")]
        public string DRE { get; set; }
        [Name("esc_eol")]
        public string ESC_EOL { get; set; }
        [Name("ano_escolar")]
        public string Ano_Escolar { get; set; }
        [Name("turma")]
        public string Turma { get; set; }
        [Name("nome_arquivo")]
        public string Desc_Arquivo { get; set; }
        [Name("eol_aluno")]
        public string Cod_EOL { get; set; }
        [Name("tipo_prova")]
        public string Tipo_Prova { get; set; }
        [Name("questao")]
        public string questao { get; set; }
        [Name("pagina")]
        public string pagina { get; set; }
    }
}
