using GestaoAvaliacao.Entities;
using System;
using System.Threading.Tasks;

namespace GestaoAvaliacao.IRepository
{
    public interface IStudentTestSessionRepository
    {
        Task AddAsync(StudentTestSession entity);

        Task UpdateAsync(StudentTestSession entity);

        Task<StudentTestSession> GetAsync(long id);

        Task<StudentTestSession> GetAsync(long aluId, long turId, long testId);

        Task<StudentTestSession> GetAsync(Guid connectionId);
    }
}