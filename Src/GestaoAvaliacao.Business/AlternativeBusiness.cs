using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Entities.Projections;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.IRepository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Business
{
    public class AlternativeBusiness : IAlternativeBusiness
    {
        private readonly IAlternativeRepository _alternativeRepository;
        public AlternativeBusiness(IAlternativeRepository alternativeRepository)
        {
            _alternativeRepository = alternativeRepository;
        }

        public async Task<IEnumerable<AlternativesWithNumerationAndOrderProjection>> GetAlternativesWithNumerationAndOrderByTest(long test_id)
        {
            return await _alternativeRepository.GetAlternativesWithNumerationAndOrderByTest(test_id);
        }

        public List<Alternative> GetAlternativesByItens(long[] itens, long test_id)
        {
            return _alternativeRepository.GetAlternativesByItens(itens, test_id);
        }        
    }
}
