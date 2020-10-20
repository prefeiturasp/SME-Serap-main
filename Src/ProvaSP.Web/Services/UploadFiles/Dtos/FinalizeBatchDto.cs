using ProvaSP.Web.Services.Abstractions;

namespace ProvaSP.Web.Services.UploadFiles.Dtos
{
    public class FinalizeBatchDto : BaseDto
    {
        public long Id { get; set; }
        public long FileErrorCount { get; set; }
    }
}