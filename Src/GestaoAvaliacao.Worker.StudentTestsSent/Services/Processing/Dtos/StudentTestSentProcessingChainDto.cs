using GestaoAvaliacao.Worker.Domain.Entities.Schools;
using GestaoAvaliacao.Worker.Domain.Entities.Tests;
using GestaoAvaliacao.Worker.Domain.MongoDB.Entities.Tests;
using GestaoAvaliacao.Worker.StudentTestsSent.Processing.Dtos.Base;

namespace GestaoAvaliacao.Worker.StudentTestsSent.Processing.Dtos
{
    internal class StudentTestSentProcessingChainDto : NotificableWorkerDto
    {
        public StudentTestSentEntityWorker StudentTestSent { get; set; }
        public StudentCorrectionEntityWorker StudentCorrection { get; set; }
        public SchoolEntityWorker School { get; set; }
        public TestTemplateEntityWorker TestTemplate { get; set; }
    }
}