using FluentValidation;

namespace ProvaSP.Web.Services.UploadFiles.Dtos.Validators
{
    public class StartBatchDtoValidator : AbstractValidator<StartBatchDto>
    {
        public StartBatchDtoValidator()
        {
            RuleFor(x => x.FileCount)
                .NotEmpty()
                .WithMessage("O número de arquivos deve ser maior do que 0 (zero).");

            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("O identificador do lote que será iniciado deve ser informado.");
        }
    }
}