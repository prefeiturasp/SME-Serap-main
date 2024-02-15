using ImportacaoDeQuestionariosSME.Services.CaracterizacaoFamiliasEscolasQuestionarioResposta.Dtos;
using System.Threading.Tasks;

namespace ImportacaoDeQuestionariosSME.Services.CaracterizacaoFamiliasEscolasQuestionarioResposta
{
    public interface ICaracterizacaoFamiliasEscolasQuestionarioRespostaService
    {
        Task ImportarAsync(CaracterizacaoFamiliasEscolasQuestionarioRespostaDto dto);
    }
}
