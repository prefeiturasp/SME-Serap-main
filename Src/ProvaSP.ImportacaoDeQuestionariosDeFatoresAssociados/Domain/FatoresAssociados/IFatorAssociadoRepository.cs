using System.Collections.Generic;
using System.Threading.Tasks;

namespace ImportacaoDeQuestionariosSME.Domain.FatoresAssociados
{
    public interface IFatorAssociadoRepository
    {
        Task InsertAsync(IEnumerable<FatorAssociado> fatoresAssociados);
    }
}