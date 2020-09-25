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
            => await _alternativeRepository.GetAlternativesWithNumerationAndOrderByTest(test_id);

        public List<Alternative> GetAlternativesByItens(IEnumerable<string> itens, long test_id) => _alternativeRepository.GetAlternativesByItens(itens, test_id);

        public async Task<IEnumerable<Alternative>> GetAlternativesByItensAsync(IEnumerable<string> itens, long test_id) 
            => await _alternativeRepository.GetAlternativesByItensAsync(itens, test_id);
    }
}
