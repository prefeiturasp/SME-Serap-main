using Newtonsoft.Json;

namespace GestaoAvaliacao.FGVIntegration.Models
{
    public class Rede : BaseFGVObject
    {
        public string NomeDaRede { get; set; }

        public string NomeDoResponsavel { get; set; }

        public string CargoDoResponsavel { get; set; }

        public string CPFDoResponsavel { get; set; }

        public string UF { get; set; }

        public int Cidade { get; set; }

        public string CNPJ { get; set; }

        public string WebSite { get; set; }

        public int Tipo { get; set; }
    }
}
