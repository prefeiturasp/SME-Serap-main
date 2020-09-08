using Newtonsoft.Json;

namespace GestaoAvaliacao.FGVIntegration.Models
{
    public class ResultadoFGV
    {

        [JsonProperty("seq")]
        public int Seq { get; set; }

        [JsonProperty("status")]
        public byte Status { get; set; }

        [JsonProperty("operation")]
        public string Operation { get; set; }

        [JsonProperty("errormessage")]
        public string ErrorMessage { get; set; }

        [JsonProperty("validationlist")]
        public ItemResultadoFGV[] ValidationList { get; set; }

        [JsonProperty("returns")]
        public string Returns { get; set; }

    }

}