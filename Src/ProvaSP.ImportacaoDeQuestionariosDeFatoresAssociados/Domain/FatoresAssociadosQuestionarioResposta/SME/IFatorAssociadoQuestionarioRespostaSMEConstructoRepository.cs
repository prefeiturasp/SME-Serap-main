using System.Collections.Generic;
using System.Threading.Tasks;

namespace ImportacaoDeQuestionariosSME.Domain.FatoresAssociadosQuestionarioResposta.SME
{
    public interface IFatorAssociadoQuestionarioRespostaSMEConstructoRepository
    {
        Task InsertAsync(IEnumerable<FatorAssociadoQuestionarioRespostaSMEConstructo> entities);
    }
}