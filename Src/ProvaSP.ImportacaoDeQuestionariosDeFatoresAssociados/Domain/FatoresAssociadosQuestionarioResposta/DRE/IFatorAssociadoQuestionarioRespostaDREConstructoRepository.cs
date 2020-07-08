using System.Collections.Generic;
using System.Threading.Tasks;

namespace ImportacaoDeQuestionariosSME.Domain.FatoresAssociadosQuestionarioResposta.DRE
{
    public interface IFatorAssociadoQuestionarioRespostaDREConstructoRepository
    {
        Task InsertAsync(IEnumerable<FatorAssociadoQuestionarioRespostaDREConstructo> entities);
    }
}