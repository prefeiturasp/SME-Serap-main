using GestaoAvaliacao.MongoEntities;
using GestaoAvaliacao.Worker.Domain.Entities.Schools;
using GestaoAvaliacao.Worker.Domain.Entities.Tests;
using GestaoAvaliacao.Worker.StudentTestsSent.Processing.Dtos.Base;

namespace GestaoAvaliacao.Worker.StudentTestsSent.Processing.Dtos
{
    internal class StudentTestSentProcessingChainDto : NotificableWorkerDto
    {
        public StudentTestSentEntityWorker StudentTestSent { get; set; }
        public StudentCorrection StudentCorrection { get; set; }
        public SchoolEntityWorker School { get; set; }
        public TestTemplate TestTemplate { get; set; }
    }
}