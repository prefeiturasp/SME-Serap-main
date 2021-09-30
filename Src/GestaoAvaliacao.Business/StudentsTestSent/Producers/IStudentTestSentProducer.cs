using GestaoAvaliacao.Business.StudentsTestSent.Producers.Datas;
using System;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Business.StudentsTestSent.Producers
{
    public interface IStudentTestSentProducer
    {
        void Close();
        Task<bool> PublishAsync(StudentTestSentMessageData data, Guid usuId);
    }
}