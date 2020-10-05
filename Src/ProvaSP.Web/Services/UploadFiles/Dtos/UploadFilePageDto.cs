using ProvaSP.Web.Services.Abstractions;

namespace ProvaSP.Web.Services.UploadFiles.Dtos
{
    public class UploadFilePageDto : BaseDto
    {
        public bool BatchInProgressExists { get; set; }
        public bool BatchInProgressOwner { get; set; }
    }
}