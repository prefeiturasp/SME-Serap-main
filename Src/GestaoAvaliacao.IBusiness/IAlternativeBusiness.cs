
using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Entities.Projections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestaoAvaliacao.IBusiness
{
    public interface IAlternativeBusiness
    {
        Task<IEnumerable<AlternativesWithNumerationAndOrderProjection>> GetAlternativesWithNumerationAndOrderByTest(long test_id);
        List<Alternative> GetAlternativesByItens(IEnumerable<string> itens, long test_id);
        Task<IEnumerable<Alternative>> GetAlternativesByItensAsync(IEnumerable<string> itens, long test_id);
    }
}
