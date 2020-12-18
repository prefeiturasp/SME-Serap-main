using System.Threading;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Worker.StudentTestsSent.Consumers
{
    public interface IStudentTestSentConsumer
    {
        void Close();
        Task ConsumeAsync(CancellationToken cancellationToken);
    }
}