using FluentValidation.Results;
using ProvaSP.Web.Services.Abstractions;
using System;
using System.Collections.Generic;

namespace ProvaSP.Web.Services.UploadFiles.Dtos
{
    public class UploadFileBatchDto : BaseDto
    {
        public long Id { get; set; }
        public DateTime? BeginDate { get; set; }
        public string Edicao { get; set; }
        public string AreaDeConhecimento { get; set; }
        public string CicloDeAprendizagem { get; set; }
        public string Situacao { get; set; }
        public Guid UsuId { get; set; }
        public string UsuNome { get; set; }

        public UploadFileBatchDto() : base()
        {
        }
    }
}