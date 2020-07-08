using System.Collections.Generic;
using System.Threading.Tasks;

namespace ImportacaoDeQuestionariosSME.Domain.Constructos
{
    public interface IConstructoRepository
    {
        Task InsertAsync(IEnumerable<Constructo> constructos);

        Task<int> GetMaxConstructoId();

        Task<IEnumerable<Constructo>> GetAsync(string edicao);
    }
}