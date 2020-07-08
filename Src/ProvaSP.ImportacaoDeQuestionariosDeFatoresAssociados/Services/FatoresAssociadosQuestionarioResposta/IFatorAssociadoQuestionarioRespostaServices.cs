using ImportacaoDeQuestionariosSME.Services.FatoresAssociados.Dtos;
using ImportacaoDeQuestionariosSME.Services.FatoresAssociadosQuestionarioResposta.Dtos;
using System.Threading.Tasks;

namespace ImportacaoDeQuestionariosSME.Services.FatoresAssociadosQuestionarioResposta
{
    public interface IFatorAssociadoQuestionarioRespostaServices
    {
        Task ImportarAsync(ImportacaoDeQuestionariosDeFatoresAssociadosDto dto);
    }
}