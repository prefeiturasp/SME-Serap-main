using FluentValidation;

namespace ProvaSP.Web.Services.UploadFiles.Dtos.Validators
{
    public class FinalizeBatchDtoValidator : AbstractValidator<FinalizeBatchDto>
    {
        public FinalizeBatchDtoValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("O identificador do lote que será finalizado deve ser informado.");
        }
    }
}