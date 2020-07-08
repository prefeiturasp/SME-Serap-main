using ImportacaoDeQuestionariosSME.Data.Repositories.Abstractions;
using ImportacaoDeQuestionariosSME.Domain.CiclosAnoEscolar;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ImportacaoDeQuestionariosSME.Data.Repositories.CiclosAnoEscolar
{
    public class CicloAnoEscolarRepository : BaseRepository, ICicloAnoEscolarRepository
    {
        public async Task<IEnumerable<CicloAnoEscolar>> GetAsync()
        {
            var query = "SELECT * FROM CicloAnoEscolar (NOLOCK)";
            return await _dapperContext.QueryAsync<CicloAnoEscolar>(query);
        }

        public async Task<IEnumerable<int>> GetCicloIds()
        {
            var query = "SELECT CicloId FROM Ciclo (NOLOCK)";
            return await _dapperContext.QueryAsync<int>(query);
        }
    }
}