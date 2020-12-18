using GestaoAvaliacao.Business.StudentsTestSent.Producers.Datas;
using GestaoAvaliacao.Rabbit.Producers;

namespace GestaoAvaliacao.Business.StudentsTestSent.Producers
{
    public class StudentTestSentProducer : GestaoAvaliacaoRabbitBaseProducer<StudentTestSentMessageData>, IStudentTestSentProducer
    {
    }
}