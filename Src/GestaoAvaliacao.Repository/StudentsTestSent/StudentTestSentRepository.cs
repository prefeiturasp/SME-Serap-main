using GestaoAvaliacao.Entities.StudentsTestSent;
using GestaoAvaliacao.IRepository.StudentsTestSent;
using GestaoAvaliacao.Repository.Context;
using System;
using System.Data.Entity;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Repository.StudentsTestSent
{
    public class StudentTestSentRepository : IStudentTestSentRepository
    {
        private readonly GestaoAvaliacaoContext _gestaoAvaliacaoContext;

        public StudentTestSentRepository()
        {
            _gestaoAvaliacaoContext = new GestaoAvaliacaoContext();
        }

        public async Task AddAsync(StudentTestSent entity)
        {
            if (entity is null || !entity.Validate.IsValid) return;
            _gestaoAvaliacaoContext.StudentTestsSent.Add(entity);
            await _gestaoAvaliacaoContext.SaveChangesAsync();
        }

        public Task<bool> AnyAsync(long testId, long turId, long aluId)
            => _gestaoAvaliacaoContext
                .StudentTestsSent
                .AnyAsync(x => x.TestId == testId && x.TurId == turId && x.AluId == aluId);

        public Task<StudentTestSent> GetFirstOrDefaultAsync(long testId, long turId, long aluId)
            => _gestaoAvaliacaoContext
                .StudentTestsSent
                .FirstOrDefaultAsync(x => x.TestId == testId && x.TurId == turId && x.AluId == aluId);

        public Task<StudentTestSent> GetFirstBySituationAsync(StudentTestSentSituation situation) 
            => _gestaoAvaliacaoContext
                .StudentTestsSent
                .FirstOrDefaultAsync(x => x.Situation == situation);

        public async Task RemoveAsync(StudentTestSent entity)
        {
            if (entity is null || !entity.Validate.IsValid) return;
            _gestaoAvaliacaoContext.StudentTestsSent.Remove(entity);
            await _gestaoAvaliacaoContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(StudentTestSent entity)
        {
            if (entity is null || !entity.Validate.IsValid) return;
            entity.UpdateDate = DateTime.Now;
            _gestaoAvaliacaoContext.Entry(entity).State = EntityState.Modified;
            await _gestaoAvaliacaoContext.SaveChangesAsync();
        }
    }
}