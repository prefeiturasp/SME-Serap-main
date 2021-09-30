using GestaoAvaliacao.Worker.Repository.Contracts;
using GestaoAvaliacao.Worker.StudentTestsSent.Processing.Contracts;
using GestaoAvaliacao.Worker.StudentTestsSent.Processing.Dtos;
using GestaoAvaliacao.Worker.StudentTestsSent.Services.CorrectionResult;
using GestaoAvaliacao.Worker.StudentTestsSent.Services.CorrectionResult.Dtos;
using System.Threading;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Worker.StudentTestsSent.Processing.Steps
{
    internal class GenerateCorrectionResultsStep : StudentTestSentProcessingStep
    {
        private readonly IStudentCorrectionAuxiliarRepository _studentCorrectionAuxiliarRepository;
        private readonly IGenerateCorrectionResultsServices _generateCorrectionResultsServices;

        public GenerateCorrectionResultsStep(IStudentCorrectionAuxiliarRepository studentCorrectionAuxiliarRepository,
            IGenerateCorrectionResultsServices generateCorrectionResultsServices)
        {
            _studentCorrectionAuxiliarRepository = studentCorrectionAuxiliarRepository;
            _generateCorrectionResultsServices = generateCorrectionResultsServices;
        }

        protected override async Task OnExecuting(StudentTestSentProcessingChainDto dto, CancellationToken cancellationToken)
        {
            var answersGrid = await _studentCorrectionAuxiliarRepository.GetTestQuestionsAsync(dto.StudentTestSent.TestId);
            var generateCorrectionResultsDto = new GenerateCorrectionResultsDto
            {
                AnswersGrid = answersGrid,
                DreId = dto.School.dre_id,
                EntId = dto.StudentTestSent.EntId,
                EscId = dto.School.esc_id,
                TestId = dto.StudentTestSent.TestId,
                TestTemplate = dto.TestTemplate,
                TurId = dto.StudentTestSent.TurId
            };

            await _generateCorrectionResultsServices.ExecuteAsync(generateCorrectionResultsDto, cancellationToken);
            if (!generateCorrectionResultsDto.IsValid)
            {
                dto.AddError(generateCorrectionResultsDto.Errors);
            }
        }
    }
}