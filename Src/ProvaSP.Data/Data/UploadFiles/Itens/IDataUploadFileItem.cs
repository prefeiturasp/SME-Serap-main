using ProvaSP.Model.Entidades.UploadFiles.Itens;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProvaSP.Data.Data.UploadFiles.Itens
{
    public interface IDataUploadFileItem
    {
        Task<long> GetNextIdAsync();

        Task<bool> AnyAsync(long uploadFileBatchId);

        Task AddAsync(IEnumerable<UploadFileItem> entities);
    }
}