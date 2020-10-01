using ProvaSP.Web.Services.Abstractions;
using System.Collections.Generic;

namespace ProvaSP.Web.Services.UploadFiles.Dtos.Itens
{
    public class ListUploadFileItemDto : BaseDto
    {
        public IEnumerable<UploadFileItemDto> Itens { get; set; }

        public ListUploadFileItemDto() : base()
        {
        }
    }
}