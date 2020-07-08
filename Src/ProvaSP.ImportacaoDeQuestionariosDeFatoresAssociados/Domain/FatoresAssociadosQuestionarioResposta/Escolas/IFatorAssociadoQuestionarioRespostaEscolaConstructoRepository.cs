using System.Collections.Generic;
using System.Threading.Tasks;

namespace ImportacaoDeQuestionariosSME.Domain.FatoresAssociadosQuestionarioResposta.Escolas
{
    public interface IFatorAssociadoQuestionarioRespostaEscolaConstructoRepository
    {
        Task InsertAsync(IEnumerable<FatorAssociadoQuestionarioRespostaEscolaConstructo> entities);
    }
}