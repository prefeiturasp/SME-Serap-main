using FluentValidation;
using ProvaSP.Model.Entidades.UploadFiles;

namespace ProvaSP.Web.Services.UploadFiles.Dtos.Validators
{
    public class AddUploadFileBatchDtoValidator : AbstractValidator<AddUploadFileBatchDto>
    {
        public AddUploadFileBatchDtoValidator()
        {
            RuleFor(x => x.Edicao)
                .NotEmpty()
                .WithMessage("O ano de edição deve ser informado.")
                .Matches("^[0-9]{" + UploadFileBatch.EdicaoMaxLength + "}+$")
                .WithMessage("O ano de edição informado é inválido.");

            RuleFor(x => x.AreaDeConhecimento)
                .NotEmpty()
                .WithMessage("A Área de Conhecimento deve ser informada.");

            RuleFor(x => x.CicloDeAprendizagem)
                .NotEmpty()
                .WithMessage("O Ciclo de Aprendizagem deve ser informado.");

            RuleFor(x => x.UsuId)
                .NotEmpty()
                .WithMessage("O usuário que está criando o lote deve ser informado.");
        }
    }
}