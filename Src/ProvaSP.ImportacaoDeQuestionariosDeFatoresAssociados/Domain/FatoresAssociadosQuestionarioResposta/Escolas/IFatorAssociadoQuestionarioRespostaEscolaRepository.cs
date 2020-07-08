using System.Collections.Generic;
using System.Threading.Tasks;

namespace ImportacaoDeQuestionariosSME.Domain.FatoresAssociadosQuestionarioResposta.Escolas
{
    public interface IFatorAssociadoQuestionarioRespostaEscolaRepository
    {
        Task InsertAsync(IEnumerable<FatorAssociadoQuestionarioRespostaEscola> fatoresAssociadoQuestionarioRespostaEscola);

        Task<int> GetNextPk();
    }
}