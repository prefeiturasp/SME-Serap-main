using ProvaSP.Model.Entidades.UploadFiles;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProvaSP.Data.Data.UploadFiles
{
    public interface IDataUploadFileBatch
    {
        Task<long?> AddAsync(UploadFileBatch entity);

        Task UpdateAsync(UploadFileBatch entity);

        Task<UploadFileBatch> GetAsync(long id);

        Task<UploadFileBatch> GetActiveBatchAsync(UploadFileBatchType uploadFileBatchType);

        Task<bool> AnyBatchActiveAsync(UploadFileBatchType uploadFileBatchType);
    }
}