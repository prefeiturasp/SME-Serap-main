using ProvaSP.Web.Controllers.Abstractions;
using ProvaSP.Web.Services.UploadFiles.Dtos.Search;
using ProvaSP.Web.Services.UploadFiles.RevistasEBoletins;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace ProvaSP.Web.Controllers.UploadFiles
{
    public class UploadRevistasEBoletinsHistoricoController : BaseController
    {
        private readonly IUploadRevistasEBoletinsServices _uploadFileServices;

        public UploadRevistasEBoletinsHistoricoController()
        {
            _uploadFileServices = new UploadRevistasEBoletinsServices();
        }

        [HttpGet]
        public async Task<HttpResponseMessage> Get([FromUri] UploadFileSearchDto dto)
        {
            var result = await _uploadFileServices.GetAsync(dto);
            return GetResponse(result);
        }
    }
}