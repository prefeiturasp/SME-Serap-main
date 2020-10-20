using ProvaSP.Model.Entidades.UploadFiles;
using ProvaSP.Model.Entidades.UploadFiles.Pagination;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProvaSP.Data.Data.UploadFiles
{
    public interface IDataUploadFileBatch
    {
        Task<long?> AddAsync(UploadFileBatch entity);

        Task UpdateAsync(UploadFileBatch entity);

        Task UpdateAsync(IEnumerable<UploadFileBatch> entities);

        Task<UploadFileBatch> GetAsync(long id);

        Task<IEnumerable<UploadFileBatch>> GetActiveBatchesAsync(Guid usuId, UploadFileBatchType uploadFileBatchType);

        Task<bool> AnyBatchActiveAsync(UploadFileBatchType uploadFileBatchType);

        Task<UploadFileBatchPaginated> GetAsync(UploadFileBatchFilter filter);
    }
}