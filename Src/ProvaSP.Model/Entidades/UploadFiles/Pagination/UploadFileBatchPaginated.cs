using System.Collections.Generic;

namespace ProvaSP.Model.Entidades.UploadFiles.Pagination
{
    public class UploadFileBatchPaginated
    {
        public int Page { get; set; }
        public int? MaxPage { get; set; }
        public IEnumerable<UploadFileBatch> Entities { get; set; }
    }
}