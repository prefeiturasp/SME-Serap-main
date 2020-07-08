using System.Collections.Generic;
using System.Threading.Tasks;

namespace ImportacaoDeQuestionariosSME.Domain.Escolas
{
    public interface IEscolaRepository
    {
        Task<IEnumerable<Escola>> GetAsync();
    }
}