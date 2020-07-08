using ImportacaoDeQuestionariosSME.Data.Repositories.Alunos.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ImportacaoDeQuestionariosSME.Domain.Alunos
{
    public interface IAlunoRepository
    {
        Task<IEnumerable<MatriculaAnoEscolarDto>> GetAnoEscolarDoAlunoAsync(string edicao);
    }
}