using ProvaSP.Web.Controllers.Abstractions;
using ProvaSP.Web.Services.UploadFiles;
using ProvaSP.Web.Services.UploadFiles.Dtos;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace ProvaSP.Web.Controllers.UploadFiles
{
    public class UploadFileController : BaseController
    {
        private readonly IUploadFileServices _uploadFileServices;

        public UploadFileController()
        {
            _uploadFileServices = new UploadFileServices();
        }

        [HttpPost]
        public async Task<HttpResponseMessage> AddBatch([FromBody]AddUploadFileBatchDto dto)
        {
            try
            {
                var result = await _uploadFileServices.AddBatchAsync(dto);
                return GetResponse(result);
            }
            catch(Exception ex)
            {
                throw new HttpResponseException(GetErrorResponse(ex));
            }
        }
    }
}