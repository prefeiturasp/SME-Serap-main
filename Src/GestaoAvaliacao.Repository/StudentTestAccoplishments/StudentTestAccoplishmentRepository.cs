using GestaoAvaliacao.Dtos.StudentTestAccoplishments;
using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Entities.StudentTestAccoplishments;
using GestaoAvaliacao.IRepository;
using GestaoAvaliacao.Repository.Context;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Diagnostics.Eventing.Reader;
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

        public Task<List<StudentTestTimeDto>> GetAsyncByAluId(long aluId)
        {
            return _context.Database.SqlQuery<StudentTestTimeDto>($@"
                SELECT 
	                sta.AluId,
	                sta.TurId,
	                sta.Test_Id,
	                sts.StartDate, 
	                sts.EndDate
	                INTO #TempSessaoDoEstudante
                FROM StudentTestAccoplishment  sta
                INNER JOIN StudentTestSession sts ON sts.StudentTestAccoplishment_Id = sta.Id
                WHERE 
	                sts.state=1 AND 
	                sta.state=1 AND 
	                sts.Situation IN (4,3) AND
	                sta.AluId = @aluId

                SELECT 
	                TurId, 
	                Test_Id as TestId,
	                CONVERT(varchar, DATEADD(ms, SUM(DATEDIFF(SECOND, StartDate, EndDate)) * 1000, 0), 108) as TempoDeDuracao,
                    CONVERT(VARCHAR,MAX(EndDate),103) as DataDeFinalizacaoDaProva
                FROM #TempSessaoDoEstudante
                GROUP BY TurId, Test_Id
            ",new SqlParameter("aluId", aluId)).ToListAsync();
        }

        public Task<StudentTestTimeDto> GetAsyncByAluIdTurIdTestId(long aluId, long turId, long testId)
        {
            return _context.Database.SqlQuery<StudentTestTimeDto>($@"
                SELECT 
	                sts.StartDate, 
	                sts.EndDate
	                INTO #TempSessaoDoEstudante
                FROM StudentTestAccoplishment  sta
                INNER JOIN StudentTestSession sts ON sts.StudentTestAccoplishment_Id = sta.Id
                WHERE 
	                sts.state=1 AND 
	                sta.state=1 AND 
	                sts.Situation IN (4,3) AND
	                sta.AluId = @aluId AND
                    sta.TurId = @turId AND
                    sta.Test_Id = @testId

                SELECT 
	                ISNULL(SUM(DATEDIFF(SECOND, StartDate, EndDate)),0) as TempoDeDuracaoEmSegundos
                FROM #TempSessaoDoEstudante
            ", new SqlParameter("aluId", aluId), new SqlParameter("turId", turId), new SqlParameter("testId", testId)).FirstOrDefaultAsync();
        }
    }
}