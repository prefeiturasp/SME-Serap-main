using ImportacaoDeQuestionariosSME.Services.ImagensDeAlunos.Dtos;
using System.Threading.Tasks;

namespace ImportacaoDeQuestionariosSME.Services.ImagensDeAlunos
{
    public interface IImagemAlunoServices
    {
        Task ImportarAsync(ImportacaoDeImagemAlunoDto dto);
    }
}