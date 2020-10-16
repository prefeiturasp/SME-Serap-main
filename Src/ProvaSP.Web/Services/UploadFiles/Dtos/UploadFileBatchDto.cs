using ProvaSP.Web.Services.Abstractions;
using System;

namespace ProvaSP.Web.Services.UploadFiles.Dtos
{
    public class UploadFileBatchDto : BaseDto
    {
        public long Id { get; set; }
        public DateTime? BeginDate { get; set; }
        public string Edicao { get; set; }
        public string AreaDeConhecimento { get; set; }
        public string CicloDeAprendizagem { get; set; }
        public string Situation { get; set; }
        public string DirectoryPath { get; set; }
        public Guid UsuId { get; set; }

        public UploadFileBatchDto() : base()
        {
        }
    }
}