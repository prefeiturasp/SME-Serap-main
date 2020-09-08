using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GestaoAvaliacao.FGVIntegration.Models
{
    public class ResultadoRequestFGV
    {

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("returns")]
        public JContainer Returns { get; set; }

    }

}