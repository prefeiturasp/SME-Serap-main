using GestaoAvaliacao.MongoEntities;
using GestaoAvaliacao.Worker.Repository.MongoDB.Contracts;
using GestaoAvaliacao.Worker.StudentTestsSent.Processing.Contracts;
using GestaoAvaliacao.Worker.StudentTestsSent.Processing.Dtos;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Worker.StudentTestsSent.Processing.Steps
{
    internal class ValidateEmptyAnswersStep : StudentTestSentProcessingStep
    {
        private readonly IStudentCorrectionMongoDBRepository _studentCorrectionMongoDBRepository;

        public ValidateEmptyAnswersStep(IStudentCorrectionMongoDBRepository studentCorrectionMongoDBRepository)
        {
            _studentCorrectionMongoDBRepository = studentCorrectionMongoDBRepository;
        }

        protected override async Task OnExecuting(StudentTestSentProcessingChainDto dto, CancellationToken cancellationToken)
        {
            if (dto.TestTemplate is null)
            {
                dto.AddError("Não foi possível localizar o template da prova.");
                return;
            }

            var answeredItensIds = dto.StudentCorrection.Answers.Select(x => x.Item_Id).ToList();
            var templateItensIds = dto.TestTemplate.Items.Select(x => x.Item_Id).ToList();
            var emptyItensIds = templateItensIds.Except(answeredItensIds);
            if (!emptyItensIds.Any()) return;

            var emptyAnswers = emptyItensIds
                .Select(x => new Answer
                {
                    AnswerChoice = 0,
                    Correct = false,
                    Item_Id = x,
                    Empty = true,
                    StrikeThrough = false,
                    Automatic = true
                }).ToList();

            dto.StudentCorrection.Answers.AddRange(emptyAnswers);
            dto.StudentCorrection.provaFinalizada = true;
            await _studentCorrectionMongoDBRepository.InsertOrReplaceAsync(dto.StudentCorrection, cancellationToken);
        }
    }
}