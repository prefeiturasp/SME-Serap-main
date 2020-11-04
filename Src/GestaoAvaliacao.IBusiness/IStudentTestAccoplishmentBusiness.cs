using GestaoAvaliacao.Entities.DTO.StudentTestAccoplishments;
using System.Threading.Tasks;

namespace GestaoAvaliacao.IBusiness
{
    public interface IStudentTestAccoplishmentBusiness
    {
        Task StartSessionAsync(StartStudentTestSessionDto dto);

        Task EndSessionAsync(EndStudentTestSessionDto dto);

        Task EndStudentTestAccoplishmentAsync(EndStudentTestAccoplishmentDto dto);
    }
}