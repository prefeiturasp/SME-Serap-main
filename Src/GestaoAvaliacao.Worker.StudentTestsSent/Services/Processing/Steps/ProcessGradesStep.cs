using GestaoAvaliacao.MongoEntities;
using GestaoAvaliacao.Worker.StudentTestsSent.Processing.Contracts;
using GestaoAvaliacao.Worker.StudentTestsSent.Processing.Dtos;
using GestaoAvaliacao.Worker.StudentTestsSent.Services.Grades;
using GestaoAvaliacao.Worker.StudentTestsSent.Services.Grades.Dtos;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Worker.StudentTestsSent.Processing.Steps
{
    internal class ProcessGradesStep : StudentTestSentProcessingStep
    {
        private readonly IProcessGradesServices _processGradesServices;

        public ProcessGradesStep(IProcessGradesServices processGradesServices)
        {
            _processGradesServices = processGradesServices;
        }

        protected override async Task OnExecuting(StudentTestSentProcessingChainDto dto, CancellationToken cancellationToken)
        {
            var procressGradesDto = new ProcessGradesDto
            {
                DreId = dto.School.dre_id,
                EntId = dto.StudentTestSent.EntId,
                EscId = dto.School.esc_id,
                StudentCorrections = new List<StudentCorrection> { dto.StudentCorrection },
                QuantidadeDeAlunos = 1,
                TestId = dto.StudentTestSent.TestId,
                TestTemplate = dto.TestTemplate,
                TurId = dto.StudentTestSent.TurId
            };

            await _processGradesServices.ExecuteAsync(procressGradesDto, cancellationToken);
            if (!procressGradesDto.IsValid)
            {
                dto.AddError(procressGradesDto.Errors);
            }
        }
    }
}