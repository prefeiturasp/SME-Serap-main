using GestaoAvaliacao.Entities.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestaoAvaliacao.IBusiness
{
    public interface IStudentTestSessionBusiness
    {
        Task<List<StudentTestSessionDto>> GetStudentTestSession(long test_id, long tur_id);
    }
}
