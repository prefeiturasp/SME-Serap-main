using System.Threading;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Worker.StudentTestsSent.Consumers
{
    public interface IStudentTestSentConsumer
    {
        Task ConsumeAsync(CancellationToken cancellationToken);
    }
}