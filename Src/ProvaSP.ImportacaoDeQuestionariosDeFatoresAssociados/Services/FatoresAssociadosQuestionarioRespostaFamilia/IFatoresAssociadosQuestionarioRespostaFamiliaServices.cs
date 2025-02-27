using ImportacaoDeQuestionariosSME.Services.FatoresAssociadosQuestionarioRespostaFamilia.Dtos;
using System.Threading.Tasks;

namespace ImportacaoDeQuestionariosSME.Services.FatoresAssociadosQuestionarioRespostaFamilia
{
    public interface IFatoresAssociadosQuestionarioRespostaFamiliaServices
    {
        Task ImportarAsync(ImportacaoDeQuestionariosDeFatoresAssociadosFamiliaDto dto);
    }
}