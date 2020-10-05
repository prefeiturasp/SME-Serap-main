using ProvaSP.Web.Services.UploadFiles.Dtos;
using System;
using System.Threading.Tasks;

namespace ProvaSP.Web.Services.UploadFiles.RevistasEBoletins
{
    public interface IUploadRevistasEBoletinsServices
    {
        Task<UploadFileBatchDto> AddBatchAsync(AddUploadFileBatchDto dto);

        Task<UploadFilePageDto> LoadAsync(Guid usuId);
    }
}