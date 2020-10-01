using System;

namespace ProvaSP.Web.Services.UploadFiles.Dtos
{
    public class AddUploadFileBatchDto
    {
        public string Edicao { get; set; }
        public short AreaDeConhecimento { get; set; }
        public short CicloDeAprendizagem { get; set; }
        public Guid UsuId { get; set; }
    }
}