using System.Collections.Generic;
using System.Threading.Tasks;

namespace ImportacaoDeQuestionariosSME.Domain.FatoresAssociadosQuestionario
{
    public interface IFatorAssociadoQuestionarioRepository
    {
        Task InsertAsync(IEnumerable<FatorAssociadoQuestionario> fatoresAssociadosQuestionarios);
    }
}