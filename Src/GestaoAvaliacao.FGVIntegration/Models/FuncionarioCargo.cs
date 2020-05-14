using Newtonsoft.Json;

namespace GestaoAvaliacao.FGVIntegration.Models
{
    public class FuncionarioCargo : BaseFGVObject
    {

        [JsonProperty("codigoRF")]
        public string CodigoRF { get; set; }

        [JsonProperty("nomeServidor")]
        public string NomeServidor { get; set; }

        [JsonProperty("dataInicio")]
        public string DataInicio { get; set; }

        [JsonProperty("dataFim")]
        public string DataFim { get; set; }

        [JsonProperty("cargo")]
        public string Cargo { get; set; }

    }
}