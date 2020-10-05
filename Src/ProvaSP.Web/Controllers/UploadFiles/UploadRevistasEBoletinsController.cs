using ProvaSP.Web.Controllers.Abstractions;
using ProvaSP.Web.Services.UploadFiles.Dtos;
using ProvaSP.Web.Services.UploadFiles.RevistasEBoletins;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace ProvaSP.Web.Controllers.UploadFiles
{
    public class UploadRevistasEBoletinsController : BaseController
    {
        private readonly IUploadRevistasEBoletinsServices _uploadFileServices;

        public UploadRevistasEBoletinsController()
        {
            _uploadFileServices = new UploadRevistasEBoletinsServices();
        }

        [HttpGet]
        public async Task<HttpResponseMessage> Load(Guid usuId)
        {
            var result = await _uploadFileServices.LoadAsync(usuId);
            return GetResponse(result);
        }

        [HttpPost]
        public async Task<HttpResponseMessage> AddBatch([FromBody] AddUploadFileBatchDto dto)
        {
            try
            {
                var result = await _uploadFileServices.AddBatchAsync(dto);
                return GetResponse(result);
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(GetErrorResponse(ex));
            }
        }
    }
}