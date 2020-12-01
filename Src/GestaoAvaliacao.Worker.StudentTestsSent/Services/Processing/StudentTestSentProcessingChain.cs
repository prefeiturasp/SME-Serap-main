using GestaoAvaliacao.Worker.Repository.Contracts;
using GestaoAvaliacao.Worker.Repository.MongoDB.Contracts;
using GestaoAvaliacao.Worker.StudentTestsSent.Processing.Contracts;
using GestaoAvaliacao.Worker.StudentTestsSent.Processing.Dtos;
using GestaoAvaliacao.Worker.StudentTestsSent.Processing.Steps;
using GestaoAvaliacao.Worker.StudentTestsSent.Services.CorrectionResult;
using GestaoAvaliacao.Worker.StudentTestsSent.Services.Grades;
using System.Threading;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Worker.StudentTestsSent.Processing
{
    internal class StudentTestSentProcessingChain : IStudentTestSentProcessingChain
    {
        private readonly IStudentTestSentProcessingStep _startingProcess;

        public StudentTestSentProcessingChain(IStudentCorrectionMongoDBRepository studentCorrectionMongoDBRepository,
            ITestTemplateMongoDBRepository testTemplateMongoDBRepository,
            IStudentCorrectionAuxiliarRepository studentCorrectionAuxiliarRepository,
            IProcessGradesServices processGradesServices, IGenerateCorrectionResultsServices generateCorrectionResultsServices)
        {
            IStudentTestSentProcessingStep getDataToProcessStep = new GetDataToProcessStep(studentCorrectionMongoDBRepository, testTemplateMongoDBRepository, studentCorrectionAuxiliarRepository);
            IStudentTestSentProcessingStep validateEmptyAnswersStep = new ValidateEmptyAnswersStep(studentCorrectionMongoDBRepository);
            IStudentTestSentProcessingStep processGradesStep = new ProcessGradesStep(processGradesServices);
            IStudentTestSentProcessingStep generateCorrectionResultsStep = new GenerateCorrectionResultsStep(studentCorrectionAuxiliarRepository, generateCorrectionResultsServices);

            processGradesStep.SetNextStep(generateCorrectionResultsStep);
            validateEmptyAnswersStep.SetNextStep(processGradesStep);
            getDataToProcessStep.SetNextStep(validateEmptyAnswersStep);
            _startingProcess = getDataToProcessStep;
        }

        public Task ExecuteAsync(StudentTestSentProcessingChainDto dto, CancellationToken cancellationToken)
            => _startingProcess.ExecuteAsync(dto, cancellationToken);
    }
}