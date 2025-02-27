using ImportacaoDeQuestionariosSME.Domain.Enums;

namespace ImportacaoDeQuestionariosSME.Services.CaracterizacaoFamiliasEscolasQuestionarioResposta.Factories
{
    public interface ICaracterizacaoFamiliasEscolasQuestionarioRespostaFactory
    {
        ICaracterizacaoFamiliasEscolasQuestionarioRespostaService Create(TipoQuestionarioEnum tipoQuestionario);
    }
}
