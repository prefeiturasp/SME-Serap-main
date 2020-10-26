using FluentValidation;
using GestaoAvaliacao.Entities.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Business.Validators
{
    public class StartStudentTestSessionValidator : AbstractValidator<StartStudentTestSessionDto>
    {
        public StartStudentTestSessionValidator()
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

            RuleFor(x => x.UsuId)
                .NotEmpty()
                .WithMessage("A identificação do usuário logado deve ser informada.");
        }
    }
}
