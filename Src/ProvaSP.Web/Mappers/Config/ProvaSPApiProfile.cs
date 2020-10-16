using AutoMapper;
using ProvaSP.Model.Entidades.UploadFiles;
using ProvaSP.Web.Mappers.UploadFiles;
using ProvaSP.Web.Services.UploadFiles.Dtos;
using System.Collections.Generic;

namespace ProvaSP.Web.Mappers.Config
{
    public class ProvaSPApiProfile : Profile
    {
        public ProvaSPApiProfile()
        {
            CreateMap<UploadFileBatch, UploadFileBatchDto>()
                .ConvertUsing(new UploadFileBatchToDtoConverter());
        }
    }
}