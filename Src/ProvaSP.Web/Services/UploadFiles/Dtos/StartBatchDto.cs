using ProvaSP.Web.Services.Abstractions;

namespace ProvaSP.Web.Services.UploadFiles.Dtos
{
    public class StartBatchDto : BaseDto
    {
        public long Id { get; set; }
        public int FileCount { get; set; }
    }
}