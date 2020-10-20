using ProvaSP.Web.Services.Abstractions;
using System.Collections.Generic;

namespace ProvaSP.Web.Services.UploadFiles.Dtos.Search
{
    public class UploadFileSearchPageDto : BaseDto
    {
        public int Page { get; set; }
        public int? MaxPage { get; set; }
        public IEnumerable<UploadFileSeachtemDto> Itens { get; set; }

        public UploadFileSearchPageDto()
        {
            Itens = new List<UploadFileSeachtemDto>();
        }
    }
}