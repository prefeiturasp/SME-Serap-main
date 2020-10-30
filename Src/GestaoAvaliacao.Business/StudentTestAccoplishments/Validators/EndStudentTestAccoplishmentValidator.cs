using FluentValidation;
using GestaoAvaliacao.Entities.DTO.StudentTestAccoplishments;

namespace GestaoAvaliacao.Business.StudentTestAccoplishments.Validators
{
    public class EndStudentTestAccoplishmentValidator : AbstractValidator<EndStudentTestAccoplishmentDto>
    {
        public EndStudentTestAccoplishmentValidator()
        {
            RuleFor(x => x.AluId)
                .NotEmpty()
                .WithMessage("A identificação do aluno deve ser informada.");

            RuleFor(x => x.ConnectionId)
                .NotEmpty()
                .WithMessage("A identificação da conexão deve ser informada.");

            RuleFor(x => x.TestId)
                .NotEmpty()
                .WithMessage("A prova em andamento deve ser informada.");

            RuleFor(x => x.TurId)
                .NotEmpty()
                .WithMessage("A identificação da turma do aluno deve ser informada.");
        }
    }
}