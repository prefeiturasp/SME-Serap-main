using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ImportacaoDeQuestionariosSME.Domain.FatoresAssociadosQuestionarioResposta.DRE
{
    public interface IFatorAssociadoQuestionarioRespostaDRERepository
    {
        Task InsertAsync(IEnumerable<FatorAssociadoQuestionarioRespostaDRE> fatoresAssociadoQuestionarioRespostaDRE);
        Task<int> GetNextPk();
    }
}