using ImportacaoDeQuestionariosSME.Services.FatoresAssociadosQuestionarioResposta.DRE;
using ImportacaoDeQuestionariosSME.Services.FatoresAssociadosQuestionarioResposta.DRE.Dtos;
using ImportacaoDeQuestionariosSME.Services.FatoresAssociadosQuestionarioResposta.Dtos;
using ImportacaoDeQuestionariosSME.Services.FatoresAssociadosQuestionarioResposta.Escolas;
using ImportacaoDeQuestionariosSME.Services.FatoresAssociadosQuestionarioResposta.SME;
using System.Collections.Generic;
using System.Data;

namespace ImportacaoDeQuestionariosSME.Services.FatoresAssociadosQuestionarioResposta.Factory
{
    public class ImportacaoDeQuestionariosDeFatoresAssociadosFactory : IImportacaoDeQuestionariosDeFatoresAssociadosFactory
    {
        public IFatorAssociadoQuestionarioRespostaServices Create(bool isDRE, bool isEscola, bool isSME, IEnumerable<QuestaoConstructoDto> questaoConstructoDtos)
        {
            if (isDRE) return new FatorAssociadoQuestionarioRespostaDREServices(questaoConstructoDtos);
            if (isEscola) return new FatorAssociadoQuestionarioRespostaEscolaServices(questaoConstructoDtos);
            if (isSME) return new FatorAssociadoQuestionarioRespostaSMEServices(questaoConstructoDtos);
            return null;
        }
    }
}