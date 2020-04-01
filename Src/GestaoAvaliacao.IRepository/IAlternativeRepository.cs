using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Entities.Projections;
using GestaoAvaliacao.Util;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestaoAvaliacao.IRepository
{
    public interface IAlternativeRepository
    {
        Task<IEnumerable<AlternativesWithNumerationAndOrderProjection>> GetAlternativesWithNumerationAndOrderByTest(long test_id);
        List<Alternative> GetAlternativesByItens(long[] itens, long test_id);
        Alternative Save(Alternative entity);
        void Update(Alternative entity);
        Alternative Get(long id);
        IEnumerable<Alternative> GetAlternativesByItem(long item_id);
        IEnumerable<Alternative> Load(ref Pager pager);
        void Delete(long id);
    }
}
