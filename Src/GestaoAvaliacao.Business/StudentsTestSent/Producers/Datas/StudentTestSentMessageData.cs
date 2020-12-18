namespace GestaoAvaliacao.Business.StudentsTestSent.Producers.Datas
{
    public class StudentTestSentMessageData
    {
        public long TestId { get; private set; }
        public long TurId { get; private set; }
        public long AluId { get; private set; }

        public StudentTestSentMessageData(long testId, long turId, long aluId)
        {
            TestId = testId;
            TurId = turId;
            AluId = aluId;
        }
    }
}