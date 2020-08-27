using System.Collections.Generic;
using System.Threading.Tasks;

namespace ImportacaoDeQuestionariosSME.Domain.ImagensAlunos
{
    public interface IImagemAlunoRepository
    {
        Task InsertAsync(IEnumerable<ImagemAluno> imagensAlunos);
    }
}