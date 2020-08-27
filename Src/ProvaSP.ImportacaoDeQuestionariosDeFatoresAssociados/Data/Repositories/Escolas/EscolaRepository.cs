using ImportacaoDeQuestionariosSME.Data.Repositories.Abstractions;
using ImportacaoDeQuestionariosSME.Domain.Escolas;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ImportacaoDeQuestionariosSME.Data.Repositories.Escolas
{
    public class EscolaRepository : BaseRepository, IEscolaRepository
    {
        public EscolaRepository()
            : base()
        {
        }

        public async Task<IEnumerable<Escola>> GetAsync()
        {
            var query = "SELECT esc_codigo AS EscCodigo, uad_codigo AS UadCodigo FROM Escola (NOLOCK)";
            return await _dapperContext.QueryAsync<Escola>(query);
        }
    }
}