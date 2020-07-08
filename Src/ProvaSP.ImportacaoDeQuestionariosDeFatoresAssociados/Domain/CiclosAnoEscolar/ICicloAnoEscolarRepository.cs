using System.Collections.Generic;
using System.Threading.Tasks;

namespace ImportacaoDeQuestionariosSME.Domain.CiclosAnoEscolar
{
    public interface ICicloAnoEscolarRepository
    {
        Task<IEnumerable<CicloAnoEscolar>> GetAsync();

        Task<IEnumerable<int>> GetCicloIds();
    }
}