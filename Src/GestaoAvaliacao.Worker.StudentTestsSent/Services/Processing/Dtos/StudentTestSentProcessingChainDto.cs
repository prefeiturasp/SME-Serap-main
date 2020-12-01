using GestaoAvaliacao.Entities.DTO;
using GestaoAvaliacao.Entities.StudentsTestSent;
using GestaoAvaliacao.MongoEntities;
using GestaoAvaliacao.Worker.StudentTestsSent.Processing.Dtos.Base;

namespace GestaoAvaliacao.Worker.StudentTestsSent.Processing.Dtos
{
    internal class StudentTestSentProcessingChainDto : NotificableWorkerDto
    {
        public StudentTestSent StudentTestSent { get; set; }
        public StudentCorrection StudentCorrection { get; set; }
        public SchoolDTO School { get; set; }
        public TestTemplate TestTemplate { get; set; }
    }
}