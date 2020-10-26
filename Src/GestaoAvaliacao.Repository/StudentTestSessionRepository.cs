using GestaoAvaliacao.Entities;
using GestaoAvaliacao.IRepository;
using GestaoAvaliacao.Repository.Context;
using System;
using System.Data;
using System.Data.Entity;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Repository
{
    public class StudentTestSessionRepository : ConnectionReadOnly, IStudentTestSessionRepository
    {
        private readonly GestaoAvaliacaoContext _context;
        private IDbConnection _dbConnection;

        public StudentTestSessionRepository()
        {
            _context = new GestaoAvaliacaoContext();
        }

        public async Task AddAsync(StudentTestSession entity)
        {
            if (!entity?.Validate.IsValid ?? true) return;

            _context.StudentTestSessions.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(StudentTestSession entity)
        {
            if (!entity?.Validate.IsValid ?? true) return;

            entity.UpdateDate = DateTime.Now;
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public Task<StudentTestSession> GetAsync(long id) => _context.StudentTestSessions.FirstOrDefaultAsync(x => x.Id == id);

        public Task<StudentTestSession> GetAsync(long aluId, long turId, long testId)
            => _context.StudentTestSessions
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.AluId == aluId && x.TurId == turId && x.TestId == testId);

        public Task<StudentTestSession> GetAsync(Guid connectionId) 
            => _context.StudentTestSessions
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.ConnectionId == connectionId);

        private IDbConnection GetSqlConnection()
        {
            _dbConnection = _dbConnection ?? Connection;
            if (_dbConnection.State != ConnectionState.Open) _dbConnection.Open();
            return _dbConnection;
        }
    }
}