using ProvaSP.Web.Services.Abstractions;

namespace ProvaSP.Web.Services.UploadFiles.Dtos.Files
{
    public class UploadFileItemDto : BaseDto
    {
        public long UploadFileBatchId { get; set; }
        public string DirectoryPath { get; set; }
    }
}