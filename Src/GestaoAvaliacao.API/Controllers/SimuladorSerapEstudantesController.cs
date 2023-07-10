using GestaoAvaliacao.Entities;
using GestaoAvaliacao.API.Middleware;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.Util;
using GestaoAvaliacao.WebProject.Facade;
using System;
using System.Net;
using EntityFile = GestaoAvaliacao.Entities.File;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using GestaoAvaliacao.Dtos.SimuladorSerapEstudantes;

namespace GestaoAvaliacao.API.Controllers
{
    [Aut]
    [RoutePrefix("api/SimuladorSerapEstudantes")]
    public class SimuladorSerapEstudantesController : ApiController
    {
        private readonly IFileBusiness fileBusiness;

        public SimuladorSerapEstudantesController(IFileBusiness fileBusiness)
        {
            this.fileBusiness = fileBusiness;
        }

        [Route("File/Upload")]
        [HttpPost]
        [ResponseType(typeof(ResponseUploadArquivoDto))]
        public HttpResponseMessage Upload(Uploader file)
        {
            var entity = new EntityFile();

            try
            {
                var upload = new UploadModel
                {
                    ContentLength = file.ContentLength,
                    ContentType = file.ContentType,
                    InputStream = file.InputStream,
                    Stream = null,
                    FileName = file.FileName,
                    VirtualDirectory = ApplicationFacade.VirtualDirectorySme,
                    PhysicalDirectory = ApplicationFacade.PhysicalDirectorySme,
                    FileType = file.FileType,
                    UsuId = null
                };

                entity = fileBusiness.Upload(upload);
            }
            catch (Exception ex)
            {
                entity.Validate.IsValid = false;
                entity.Validate.Type = ValidateType.error.ToString();
                entity.Validate.Message = "Erro ao realizar o upload da imagem.";
                LogFacade.SaveError(ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK, new ResponseUploadArquivoDto
            {
                Success = entity.Validate.IsValid,
                Type = entity.Validate.Type,
                Message = entity.Validate.Message,
                FileLink = entity.Path,
                IdFile = entity.Id
            });
        }
    }
}