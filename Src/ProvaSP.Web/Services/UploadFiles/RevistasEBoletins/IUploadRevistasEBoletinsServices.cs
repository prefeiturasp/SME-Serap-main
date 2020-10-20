using ProvaSP.Web.Services.UploadFiles.Dtos;
using ProvaSP.Web.Services.UploadFiles.Dtos.Files;
using ProvaSP.Web.Services.UploadFiles.Dtos.Search;
using System.Threading.Tasks;
using System.Web;

namespace ProvaSP.Web.Services.UploadFiles.RevistasEBoletins
{
    public interface IUploadRevistasEBoletinsServices
    {
        Task<UploadFileBatchDto> AddBatchAsync(AddUploadFileBatchDto dto);
        Task<StartBatchDto> StartBatchAsync(StartBatchDto dto);
        Task<CancelBatchDto> CancelBatchAsync(CancelBatchDto dto);
        Task<CancelOpenedBatchesDto> CancelActiveBatchesAsync(CancelOpenedBatchesDto dto);
        Task<UploadFileItemDto> UploadFileAsync(UploadFileItemDto dto, HttpPostedFile httpPostedFile);
        Task<FinalizeBatchDto> FinalizeBatchAsync(FinalizeBatchDto dto);
        Task<UploadFileSearchPageDto> GetAsync(UploadFileSearchDto dto);
    }
}