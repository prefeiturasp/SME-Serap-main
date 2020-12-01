using GestaoAvaliacao.Entities.StudentsTestSent;
using System.Threading;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Worker.Repository.Contracts
{
    // Interface alocada junto à implementação do repositório inicialmente visando facilitar uma possível migração
    public interface IStudentTestSentRepository
    {
        Task<StudentTestSent> GetFirstToProcessAsync(CancellationToken cancellationToken);

        Task UpdateAsync(StudentTestSent entity, CancellationToken cancellationToken);
    }
}