using System;

namespace ProvaSP.Web.Services.UploadFiles.Dtos.Search
{
    public class UploadFileSeachtemDto
    {
        public long UploadFileBatchId { get; set; }
        public string UserName { get; set; }
        public string CreatedDate { get; set; }
        public string Edicao { get; set; }
        public string AreaDeConhecimento { get; set; }
        public string CicloDeAprendizagem { get; set; }
        public string Situation { get; set; }
        public long FileCount { get; set; }
        public long FileErrorCount { get; set; }
    }
}