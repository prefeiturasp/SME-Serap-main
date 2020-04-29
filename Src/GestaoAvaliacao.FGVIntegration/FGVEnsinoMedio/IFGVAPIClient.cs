using GestaoAvaliacao.FGVIntegration.Models;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace GestaoAvaliacao.FGVIntegration.FGVEnsinoMedio
{
    public interface IFGVAPIClient
    {
        Task<ResultadoFGV> SendPost(string pEndPoint, BaseFGVObject pParam);

        Task<JObject> Authenticate();

    }
}