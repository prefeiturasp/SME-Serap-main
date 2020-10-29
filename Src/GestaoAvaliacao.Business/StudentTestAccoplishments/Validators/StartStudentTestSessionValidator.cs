using FluentValidation;
using GestaoAvaliacao.Entities.DTO.StudentTestAccoplishments;

namespace GestaoAvaliacao.Business.StudentTestAccoplishments.Validators
{
    public class StartStudentTestSessionValidator : AbstractValidator<StartStudentTestSessionDto>
    {
        private const string baseMessage = "Sessão não iniciada.";

        public StartStudentTestSessionValidator()
        {
            RuleFor(x => x.AluId)
                .NotEmpty()
                .WithMessage($"{baseMessage} A identificação do aluno deve ser informada.");

            RuleFor(x => x.ConnectionId)
                .NotEmpty()
                .WithMessage($"{baseMessage} A identificação da conexão deve ser informada.");

            RuleFor(x => x.TestId)
                .NotEmpty()
                .WithMessage($"{baseMessage} A prova em andamento deve ser informada.");

            RuleFor(x => x.TurId)
                .NotEmpty()
                .WithMessage($"{baseMessage} A identificação da turma do aluno deve ser informada.");
        }
    }
}