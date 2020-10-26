using GestaoAvaliacao.Entities.DTO;
using System.Threading.Tasks;

namespace GestaoAvaliacao.IBusiness
{
    public interface IStudentTestSessionBusiness
    {
        Task StartSessionAsync(StartStudentTestSessionDto dto);

        Task EndSessionAsync(EndStudentTestSessionDto dto);
    }
}