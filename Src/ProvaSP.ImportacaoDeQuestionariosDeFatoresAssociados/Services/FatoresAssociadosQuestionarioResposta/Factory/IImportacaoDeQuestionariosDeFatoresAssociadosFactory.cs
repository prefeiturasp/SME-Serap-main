using ImportacaoDeQuestionariosSME.Services.FatoresAssociadosQuestionarioResposta.DRE.Dtos;
using ImportacaoDeQuestionariosSME.Services.FatoresAssociadosQuestionarioResposta.Dtos;
using System.Collections.Generic;
using System.Data;

namespace ImportacaoDeQuestionariosSME.Services.FatoresAssociadosQuestionarioResposta.Factory
{
    public interface IImportacaoDeQuestionariosDeFatoresAssociadosFactory
    {
        IFatorAssociadoQuestionarioRespostaServices Create(bool isDRE, bool isEscola, bool isSME, IEnumerable<QuestaoConstructoDto> questaoConstructoDtos);
    }
}