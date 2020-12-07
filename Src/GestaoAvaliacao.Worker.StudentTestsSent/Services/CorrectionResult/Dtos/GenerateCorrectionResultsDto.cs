using GestaoAvaliacao.Worker.Domain.Entities.StudentCorrections;
using GestaoAvaliacao.Worker.StudentTestsSent.Processing.Dtos.Base;
using System;
using System.Collections.Generic;

namespace GestaoAvaliacao.Worker.StudentTestsSent.Services.CorrectionResult.Dtos
{
    internal class GenerateCorrectionResultsDto : NotificableWorkerDto
    {
        public long TestId { get; set; }
        public long TurId { get; set; }
        public Guid EntId { get; set; }
        public Guid DreId { get; set; }
        public int EscId { get; set; }
        public MongoEntities.TestTemplate TestTemplate { get; set; }
        public IEnumerable<StudentCorrectionAnswerGridEntityWorker> AnswersGrid { get; set; }
    }
}