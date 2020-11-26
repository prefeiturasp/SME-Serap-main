using GestaoAvaliacao.Entities.StudentsTestSent;
using GestaoAvaliacao.IBusiness.StudentsTestSent;
using GestaoAvaliacao.IRepository.StudentsTestSent;
using GestaoAvaliacao.Util;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Business.StudentsTestSent
{
    public class StudentTestSentBusiness : IStudentTestSentBusiness
    {
        private readonly IStudentTestSentRepository _studentTestSentRepository;

        public StudentTestSentBusiness(IStudentTestSentRepository studentTestSentRepository)
        {
            _studentTestSentRepository = studentTestSentRepository;
        }

        public async Task<StudentTestSent> SaveAsync(long testId, long turId, long aluId, Guid entId, EnumSYS_Visao visao, CancellationToken cancellationToken)
        {
            var studentTestSent = await _studentTestSentRepository.GetFirstOrDefaultAsync(testId, turId, aluId);
            if (studentTestSent != null) return studentTestSent;

            studentTestSent = new StudentTestSent(testId, turId, aluId, entId, visao);
            await _studentTestSentRepository.AddAsync(studentTestSent);
            return studentTestSent;
        }
    }
}