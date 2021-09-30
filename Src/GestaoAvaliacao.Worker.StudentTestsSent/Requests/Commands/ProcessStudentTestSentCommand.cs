using MediatR;

namespace GestaoAvaliacao.Worker.StudentTestsSent.Requests.Commands
{
    public class ProcessStudentTestSentCommand : IRequest
    {
        public long TestId { get; set; }
        public long TurId { get; set; }
        public long AluId { get; set; }
    }
}