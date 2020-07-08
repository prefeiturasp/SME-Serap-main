using System.Collections.Generic;
using System.Threading.Tasks;

namespace ImportacaoDeQuestionariosSME.Domain.FatoresAssociadosQuestionarioResposta.SME
{
    public interface IFatorAssociadoQuestionarioRespostaSMERepository
    {
        Task InsertAsync(IEnumerable<FatorAssociadoQuestionarioRespostaSME> fatoresAssociadoQuestionarioRespostaSME);

        Task<int> GetNextPk();
    }
}