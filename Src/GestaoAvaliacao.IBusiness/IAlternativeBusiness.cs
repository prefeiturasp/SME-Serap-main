
using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Entities.Projections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestaoAvaliacao.IBusiness
{
    public interface IAlternativeBusiness
    {
        Task<IEnumerable<AlternativesWithNumerationAndOrderProjection>> GetAlternativesWithNumerationAndOrderByTest(long test_id);
        List<Alternative> GetAlternativesByItens(long[] itens, long test_id);
       // IEnumerable<Alternative> GetAlternativesByItem(long item_id);
    }
}
