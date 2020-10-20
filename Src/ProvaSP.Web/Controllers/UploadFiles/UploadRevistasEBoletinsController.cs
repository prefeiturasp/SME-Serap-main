using Newtonsoft.Json;
using ProvaSP.Web.Controllers.Abstractions;
using ProvaSP.Web.Services.UploadFiles.Dtos;
using ProvaSP.Web.Services.UploadFiles.Dtos.Files;
using ProvaSP.Web.Services.UploadFiles.RevistasEBoletins;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
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

        [HttpPost]
        public async Task<HttpResponseMessage> CancelActiveBatches([FromBody] CancelOpenedBatchesDto dto)
        {
            var result = await _uploadFileServices.CancelActiveBatchesAsync(dto);
            return GetResponse(result);
        }

        [HttpPost]
        public async Task<HttpResponseMessage> AddBatch([FromBody] AddUploadFileBatchDto dto)
        {
            var result = await _uploadFileServices.AddBatchAsync(dto);
            return GetResponse(result);
        }

        [HttpPost]
        public async Task<HttpResponseMessage> StartBatch([FromBody] StartBatchDto dto)
        {
            var result = await _uploadFileServices.StartBatchAsync(dto);
            return GetResponse(result);
        }

        [HttpPost]
        public async Task<HttpResponseMessage> UploadFile()
        {
            try
            {
                var files = HttpContext.Current.Request.Files;
                if (files.Count <= 0)
                {
                    return new HttpResponseMessage(HttpStatusCode.ExpectationFailed)
                    {
                        ReasonPhrase = "Nenhum arquivo foi enviado."
                    };
                }

                var forms = HttpContext.Current.Request.Form;
                if (forms.Count <= 0)
                {
                    return new HttpResponseMessage(HttpStatusCode.ExpectationFailed)
                    {
                        ReasonPhrase = "As informações de detalhamento do arquivo não foram enviadas."
                    };
                }

                var dto = JsonConvert.DeserializeObject<UploadFileItemDto>(forms[0]);
                var result = await _uploadFileServices.UploadFileAsync(dto, files[0]);

                return GetResponse(result);
            }
            catch(Exception ex)
            {
                return new HttpResponseMessage(HttpStatusCode.ExpectationFailed)
                {
                    ReasonPhrase = ex.InnerException?.Message ?? ex.Message
                };
            }
        }

        [HttpPost]
        public async Task<HttpResponseMessage> CancelBatch([FromBody] CancelBatchDto dto)
        {
            var result = await _uploadFileServices.CancelBatchAsync(dto);
            return GetResponse(result);
        }

        [HttpPost]
        public async Task<HttpResponseMessage> FinalizeBatch([FromBody] FinalizeBatchDto dto)
        {
            var result = await _uploadFileServices.FinalizeBatchAsync(dto);
            return GetResponse(result);
        }
    }
}