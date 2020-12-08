using GestaoAvaliacao.Worker.StudentTestsSent.Services.Grades.Dtos;
using System.Threading;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Worker.StudentTestsSent.Services.Grades
{
    internal interface IProcessGradesServices
    {
        Task ExecuteAsync(ProcessGradesDto dto, CancellationToken cancellationToken);
    }
}