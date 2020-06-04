using GestaoAvaliacao.FGVIntegration.Models;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestaoAvaliacao.FGVIntegration.Business
{
    public interface IFGVAPIConsumer
    {
        Task<ResultadoFGV> SendPost(string pEndPoint, BaseFGVObject pParam);

        Task<bool> ValidateAuthentication();

        Task<List<ResultadoFGV>> SendPost(string pEndPoint, IEnumerable<BaseFGVObject> pParam);
    }
}