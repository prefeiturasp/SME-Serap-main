using GestaoAvaliacao.Business.StudentsTestSent.Producers;
using GestaoAvaliacao.Business.StudentsTestSent.Producers.Datas;
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
        private readonly IStudentTestSentProducer _studentTestSentProducer;

        public StudentTestSentBusiness(IStudentTestSentRepository studentTestSentRepository, IStudentTestSentProducer studentTestSentProducer)
        {
            _studentTestSentRepository = studentTestSentRepository;
            _studentTestSentProducer = studentTestSentProducer;
        }

        public async Task<StudentTestSent> SaveAsync(long testId, long turId, long aluId, Guid entId, EnumSYS_Visao visao, Guid usuId, CancellationToken cancellationToken)
        {
            var studentTestSent = await _studentTestSentRepository.GetFirstOrDefaultAsync(testId, turId, aluId, cancellationToken);
            if (studentTestSent != null)
            {
                await PublishEventMessageAsync(testId, turId, aluId, usuId);
                return studentTestSent;
            }

            studentTestSent = new StudentTestSent(testId, turId, aluId, entId, visao);
            if (!studentTestSent.Validate.IsValid) return studentTestSent;

            await _studentTestSentRepository.AddAsync(studentTestSent, cancellationToken);
            await PublishEventMessageAsync(testId, turId, aluId, usuId);
            return studentTestSent;
        }

        public Task PublishEventMessageAsync(long testId, long turId, long aluId, Guid usuId)
        {
            var data = new StudentTestSentMessageData(testId, turId, aluId);
            return _studentTestSentProducer.PublishAsync(data, usuId);
        }
    }
}