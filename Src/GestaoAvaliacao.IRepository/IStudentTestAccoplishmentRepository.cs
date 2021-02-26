using GestaoAvaliacao.Dtos.StudentTestAccoplishments;
using GestaoAvaliacao.Entities.StudentTestAccoplishments;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestaoAvaliacao.IRepository
{
    public interface IStudentTestAccoplishmentRepository
    {
        Task AddAsync(StudentTestAccoplishment entity);

        Task UpdateAsync(StudentTestAccoplishment entity);

        Task AddSessionAsync(StudentTestSession session);

        Task UpdateSessionAsync(StudentTestSession session);

        Task<StudentTestAccoplishment> GetAsync(long aluId, long turId, long testId);

        Task<StudentTestSession> GetSessionAsync(Guid connectionId);

        Task<List<StudentTestTimeDto>> GetAsyncByAluId(long aluId);
    }
}