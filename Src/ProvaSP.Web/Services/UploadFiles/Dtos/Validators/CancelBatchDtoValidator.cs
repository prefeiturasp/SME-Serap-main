using FluentValidation;

namespace ProvaSP.Web.Services.UploadFiles.Dtos.Validators
{
    public class CancelBatchDtoValidator : AbstractValidator<CancelBatchDto>
    {
        public CancelBatchDtoValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("O identificador do lote que será cancelado deve ser informado.");
        }
    }
}