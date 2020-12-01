using GestaoAvaliacao.MongoEntities;
using GestaoAvaliacao.Worker.Repository.Contracts;
using GestaoAvaliacao.Worker.Repository.MongoDB.Contracts;
using GestaoAvaliacao.Worker.StudentTestsSent.Processing.Contracts;
using GestaoAvaliacao.Worker.StudentTestsSent.Processing.Dtos;
using System.Threading;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Worker.StudentTestsSent.Processing.Steps
{
    internal class GetDataToProcessStep : StudentTestSentProcessingStep
    {
        private readonly IStudentCorrectionMongoDBRepository _studentCorrectionMongoDBRepository;
        private readonly ITestTemplateMongoDBRepository _testTemplateMongoDBRepository;
        private readonly IStudentCorrectionAuxiliarRepository _studentCorrectionAuxiliarRepository;

        public GetDataToProcessStep(IStudentCorrectionMongoDBRepository studentCorrectionMongoDBRepository, ITestTemplateMongoDBRepository testTemplateMongoDBRepository,
            IStudentCorrectionAuxiliarRepository studentCorrectionAuxiliarRepository)
        {
            _studentCorrectionMongoDBRepository = studentCorrectionMongoDBRepository;
            _testTemplateMongoDBRepository = testTemplateMongoDBRepository;
            _studentCorrectionAuxiliarRepository = studentCorrectionAuxiliarRepository;
        }

        protected override async Task OnExecuting(StudentTestSentProcessingChainDto dto, CancellationToken cancellationToken)
        {
            if (dto.StudentCorrection is null) return;

            var loadStudentCorrection = LoadStudentCorrectionAsync(dto, cancellationToken);
            var loadTesTemplate = LoadTestTemplate(dto, cancellationToken);
            var loadSchool = LoadSchool(dto, cancellationToken);
            await Task.WhenAll(loadStudentCorrection, loadTesTemplate, loadSchool);
        }

        private async Task LoadStudentCorrectionAsync(StudentTestSentProcessingChainDto dto, CancellationToken cancellationToken)
        {
            var studentCorrectionFilter = new StudentCorrection(dto.StudentTestSent.TestId, dto.StudentTestSent.TurId, dto.StudentTestSent.AluId, dto.StudentTestSent.EntId);
            dto.StudentCorrection = await _studentCorrectionMongoDBRepository.GetEntityAsync(studentCorrectionFilter, cancellationToken);
            if (dto.StudentCorrection is null)
            {
                dto.AddError("Não foi possível localizar o registro das respostas do aluno.");
            }
        }

        private async Task LoadTestTemplate(StudentTestSentProcessingChainDto dto, CancellationToken cancellationToken)
        {
            var testTemplateFilter = new TestTemplate(dto.StudentTestSent.EntId, dto.StudentTestSent.TestId);
            dto.TestTemplate = await _testTemplateMongoDBRepository.FindOneAsync(testTemplateFilter, cancellationToken);
            if (dto.TestTemplate is null)
            {
                dto.AddError("Não foi possível localizar o template da prova.");
            }
        }

        private async Task LoadSchool(StudentTestSentProcessingChainDto dto, CancellationToken cancellationToken)
        {
            dto.School = await _studentCorrectionAuxiliarRepository.GetEscIdDreIdByTeamAsync(dto.StudentTestSent.TurId);
            if (dto.School is null)
            {
                dto.AddError("Não foi possível localizar os dados da escola do aluno.");
            }
        }
    }
}