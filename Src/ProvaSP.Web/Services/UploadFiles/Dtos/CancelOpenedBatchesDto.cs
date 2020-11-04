using ProvaSP.Web.Services.Abstractions;
using System;

namespace ProvaSP.Web.Services.UploadFiles.Dtos
{
    public class CancelOpenedBatchesDto : BaseDto
    {
        public Guid UsuId { get; set; }
    }
}