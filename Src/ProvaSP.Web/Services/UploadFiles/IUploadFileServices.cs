using ProvaSP.Web.Services.UploadFiles.Dtos;
using System.Threading.Tasks;

namespace ProvaSP.Web.Services.UploadFiles
{
    public interface IUploadFileServices
    {
        Task<UploadFileBatchDto> AddBatchAsync(AddUploadFileBatchDto dto);
    }
}