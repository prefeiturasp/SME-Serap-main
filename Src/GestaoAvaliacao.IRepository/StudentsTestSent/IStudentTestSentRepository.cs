using GestaoAvaliacao.Entities.StudentsTestSent;
using GestaoAvaliacao.Util;
using System.Threading;
using System.Threading.Tasks;

namespace GestaoAvaliacao.IRepository.StudentsTestSent
{
    public interface IStudentTestSentRepository
    {
        Task AddAsync(StudentTestSent entity, CancellationToken cancellationToken);

        Task<bool> AnyAsync(long testId, long turId, long aluId, CancellationToken cancellationToken);

        Task<StudentTestSent> GetFirstOrDefaultAsync(long testId, long turId, long aluId, CancellationToken cancellationToken);

        Task RemoveAsync(StudentTestSent entity, CancellationToken cancellationToken);

        Task UpdateAsync(StudentTestSent entity, CancellationToken cancellationToken);
    }
}