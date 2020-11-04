using AutoMapper;
using ProvaSP.Model.Entidades.UploadFiles;
using ProvaSP.Model.Entidades.UploadFiles.Pagination;
using ProvaSP.Model.Utils;
using ProvaSP.Web.Services.UploadFiles.Dtos.Search;
using System.Collections.Generic;
using System.Linq;

namespace ProvaSP.Web.Mappers.UploadFiles
{
    public class UploadFileBatchPaginatedToSearchDtoConverter : ITypeConverter<UploadFileBatchPaginated, UploadFileSearchPageDto>
    {
        public UploadFileSearchPageDto Convert(UploadFileBatchPaginated source, UploadFileSearchPageDto destination, ResolutionContext context)
        {
            return new UploadFileSearchPageDto
            {
                Page = source.Page,
                MaxPage = source.MaxPage,
                Itens = source.Entities
                .Select(x => new UploadFileSeachtemDto
                {
                    AreaDeConhecimento = x.AreaDeConhecimento.GetDescription(),
                    CicloDeAprendizagem = x.CicloDeAprendizagem.GetDescription(),
                    Edicao = x.Edicao,
                    Situation = x.Situation.GetDescription(),
                    CreatedDate = x.CreatedDate.ToShortDateString(),
                    FileCount = x.FileCount,
                    FileErrorCount = x.FileErrorCount,
                    UploadFileBatchId = x.Id,
                    UserName = x.UsuName
                })
                .ToList()
            };
        }
    }
}