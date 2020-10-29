using FluentValidation;
using GestaoAvaliacao.Entities.DTO.StudentTestAccoplishments;

namespace GestaoAvaliacao.Business.StudentTestAccoplishments.Validators
{
    public class EndStudentTestSessionValidator : AbstractValidator<EndStudentTestSessionDto>
    {
        public EndStudentTestSessionValidator()
        {
            RuleFor(x => x.ConnectionId)
                .NotEmpty()
                .WithMessage("Sessão não finalizada. A identificação da conexão deve ser informada.");
        }
    }
}