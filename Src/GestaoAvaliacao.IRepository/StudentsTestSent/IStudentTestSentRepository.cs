using GestaoAvaliacao.Entities.StudentsTestSent;
using System.Threading.Tasks;

namespace GestaoAvaliacao.IRepository.StudentsTestSent
{
    public interface IStudentTestSentRepository
    {
        Task AddAsync(StudentTestSent entity);

        Task<bool> AnyAsync(long testId, long turId, long aluId);

        Task<StudentTestSent> GetFirstBySituationAsync(StudentTestSentSituation situation);

        Task<StudentTestSent> GetFirstOrDefaultAsync(long testId, long turId, long aluId);

        Task RemoveAsync(StudentTestSent entity);

        Task UpdateAsync(StudentTestSent entity);
    }
}