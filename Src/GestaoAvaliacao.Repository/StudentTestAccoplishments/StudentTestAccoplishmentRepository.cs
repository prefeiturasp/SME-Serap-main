using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Entities.StudentTestAccoplishments;
using GestaoAvaliacao.IRepository;
using GestaoAvaliacao.Repository.Context;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Repository.StudentTestAccoplishments
{
    public class StudentTestAccoplishmentRepository : ConnectionReadOnly, IStudentTestAccoplishmentRepository
    {
        private readonly GestaoAvaliacaoContext _context;
        private IDbConnection _dbConnection;

        public StudentTestAccoplishmentRepository()
        {
            _context = new GestaoAvaliacaoContext();
        }

        public async Task AddAsync(StudentTestAccoplishment entity)
        {
            if (!entity?.Validate.IsValid ?? true) return;

            entity.SetTest(await _context.Test.FirstOrDefaultAsync(x => x.Id == entity.Test_Id));
            _context.StudentTestAccoplishments.Add(entity);

            foreach(var session in entity.Sessions)
            {
                _context.StudentTestSessions.Add(session);
            }

            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(StudentTestAccoplishment entity)
        {
            if (!entity?.Validate.IsValid ?? true) return;

            entity.UpdateDate = DateTime.Now;
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task AddSessionAsync(StudentTestSession session)
        {
            if (!session?.Validate.IsValid ?? true) return;
            _context.StudentTestSessions.Add(session);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateSessionAsync(StudentTestSession session)
        {
            if (!session?.Validate.IsValid ?? true) return;

            session.UpdateDate = DateTime.Now;
            _context.Entry(session).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public Task<StudentTestAccoplishment> GetAsync(long aluId, long turId, long testId)
            => _context.StudentTestAccoplishments
            .Include(x => x.Sessions)
            .FirstOrDefaultAsync(x => x.AluId == aluId && x.TurId == turId && x.Test_Id == testId);

        public Task<List<StudentTestAccoplishment>> GetAsync(long turId, long testId)
            => _context.StudentTestAccoplishments
            .Include(x => x.Sessions)
            .Where(x => x.TurId == turId && x.Test_Id == testId).ToListAsync();

        public Task<StudentTestSession> GetSessionAsync(Guid connectionId) 
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