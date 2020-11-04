using AutoMapper;
using ProvaSP.Model.Entidades.UploadFiles;
using ProvaSP.Model.Utils;
using ProvaSP.Web.Services.UploadFiles.Dtos;

namespace ProvaSP.Web.Mappers.UploadFiles
{
    public class UploadFileBatchToDtoConverter : ITypeConverter<UploadFileBatch, UploadFileBatchDto>
    {
        public UploadFileBatchDto Convert(UploadFileBatch source, UploadFileBatchDto destination, ResolutionContext context)
        {
            return new UploadFileBatchDto
            {
                AreaDeConhecimento = source.AreaDeConhecimento.GetDescription(),
                BeginDate = source.BeginDate,
                CicloDeAprendizagem = source.CicloDeAprendizagem.GetDescription(),
                Edicao = source.Edicao,
                Id = source.Id,
                Situation = source.Situation.GetDescription(),
                DirectoryPath = source.DirectoryPath,
                UsuId = source.UsuId
            };
        }
    }
}