using ImportacaoDeQuestionariosSME.Services.Constructos.Dtos;
using System.Threading.Tasks;

namespace ImportacaoDeQuestionariosSME.Services.Constructos
{
    public interface IConstructoServices
    {
        Task ImportarAsync(ImportacaoDeConstructosDto dto);
    }
}