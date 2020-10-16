using FluentValidation;

namespace ProvaSP.Web.Services.UploadFiles.Dtos.Validators
{
    public class CancelOpenedBatchesDtoValidator : AbstractValidator<CancelOpenedBatchesDto>
    {
        public CancelOpenedBatchesDtoValidator()
        {
            RuleFor(x => x.UsuId)
                .NotEmpty()
                .WithMessage("O usuário que criou o lote que será cancelado deve ser informado.");
        }
    }
}