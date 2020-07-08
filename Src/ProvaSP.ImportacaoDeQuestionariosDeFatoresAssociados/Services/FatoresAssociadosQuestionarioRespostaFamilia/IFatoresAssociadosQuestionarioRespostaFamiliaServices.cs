using ImportacaoDeQuestionariosSME.Services.FatoresAssociadosQuestionarioResposta.Dtos;
using ImportacaoDeQuestionariosSME.Services.FatoresAssociadosQuestionarioRespostaFamilia.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ImportacaoDeQuestionariosSME.Services.FatoresAssociadosQuestionarioRespostaFamilia
{
    public interface IFatoresAssociadosQuestionarioRespostaFamiliaServices
    {
        Task ImportarAsync(ImportacaoDeQuestionariosDeFatoresAssociadosFamiliaDto dto, IEnumerable<QuestaoConstructoDto> questaoConstructoDtos);
    }
}