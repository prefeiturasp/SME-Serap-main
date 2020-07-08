using ImportacaoDeQuestionariosSME.Services.FatoresAssociados.Dtos;
using System.Data;
using System.Threading.Tasks;

namespace ImportacaoDeQuestionariosSME.Services.FatoresAssociados
{
    public interface IFatorAssociadoServices
    {
        Task ImportarAsync(ImportacaoDeFatoresAssociadosDto dto);

        DataTable GetTabelaDeFatoresAssociadosAjustada(string caminhoDaPlanilha);
    }
}