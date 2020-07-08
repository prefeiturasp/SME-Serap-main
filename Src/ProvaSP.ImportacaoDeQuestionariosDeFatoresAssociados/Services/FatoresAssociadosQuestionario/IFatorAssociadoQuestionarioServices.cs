using ImportacaoDeQuestionariosSME.Services.FatoresAssociadosQuestionario.Dtos;
using System.Threading.Tasks;

namespace ImportacaoDeQuestionariosSME.Services.FatoresAssociadosQuestionario
{
    public interface IFatorAssociadoQuestionarioServices
    {
        Task ImportarAsync(ImportacaoDeFatorAssociadoQuestionarioDto dto);
    }
}