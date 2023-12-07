using System;
using ImportacaoDeQuestionariosSME.Domain.Enums;
using ImportacaoDeQuestionariosSME.Services.CaracterizacaoFamiliasEscolasQuestionarioResposta.DRE;
using ImportacaoDeQuestionariosSME.Services.CaracterizacaoFamiliasEscolasQuestionarioResposta.Escola;
using ImportacaoDeQuestionariosSME.Services.CaracterizacaoFamiliasEscolasQuestionarioResposta.SME;

namespace ImportacaoDeQuestionariosSME.Services.CaracterizacaoFamiliasEscolasQuestionarioResposta.Factories
{
    public class CaracterizacaoFamiliasEscolasQuestionarioRespostaFactory : ICaracterizacaoFamiliasEscolasQuestionarioRespostaFactory
    {
        public ICaracterizacaoFamiliasEscolasQuestionarioRespostaService Create(TipoQuestionarioEnum tipoQuestionario)
        {
            switch (tipoQuestionario)
            {
                case TipoQuestionarioEnum.TipoQuestionarioSme:
                    return new CaracterizacaoFamiliasEscolasQuestionarioRespostaSmeService();
                case TipoQuestionarioEnum.TipoQuestionarioDre:
                    return new CaracterizacaoFamiliasEscolasQuestionarioRespostaDreService();
                case TipoQuestionarioEnum.TipoQuestionarioEscola:
                    return new CaracterizacaoFamiliasEscolasQuestionarioRespostaEscolaService();
                case TipoQuestionarioEnum.Nenhum:
                default:
                    throw new NotImplementedException("Tipo de Questionário não implementado");
            }
        }
    }
}
