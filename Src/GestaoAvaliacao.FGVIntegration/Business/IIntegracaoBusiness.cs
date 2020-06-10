using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestaoAvaliacao.FGVIntegration.Business
{
    public interface IIntegracaoBusiness
    {
        Task<bool> RealizarIntegracaoEnsinoMedio();

        Task<bool> RealizarIntegracaoDaRede(ICollection<string> pCodigoEscolas);

    }
}