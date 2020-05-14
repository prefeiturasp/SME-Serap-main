using GestaoAvaliacao.FGVIntegration.Models;
using System.Threading.Tasks;

namespace GestaoAvaliacao.FGVIntegration.Business
{
    public interface IFGVAPIConsumer
    {
        Task<ResultadoFGV> SendPost(string pEndPoint, BaseFGVObject pParam);
        Task<bool> ValidateAuthentication();
    }
}