using GestaoAvaliacao.API.Middleware;
using GestaoAvaliacao.Dtos.ItemApi;
using GestaoAvaliacao.Dtos.SimuladorSerapEstudantes;
using GestaoAvaliacao.Entities;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.Util;
using GestaoAvaliacao.Util.Videos;
using GestaoAvaliacao.WebProject.Facade;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using EntityFile = GestaoAvaliacao.Entities.File;

namespace GestaoAvaliacao.API.Controllers
{
    [AutAttribute]
    public class ItemController : ApiController
    {
        private readonly IItemBusiness itemBusiness;
        private readonly IFileBusiness fileBusiness;
        private readonly IVideoConverter videoConverter;
        private readonly IParameterBusiness parameterBusiness;
        private const string VideoConvertedContentType = "video/webm";

        public ItemController(IItemBusiness itemBusiness, IFileBusiness fileBusiness, IVideoConverter videoConverter, IParameterBusiness parameterBusiness)
        {
            this.itemBusiness = itemBusiness;
            this.fileBusiness = fileBusiness;
            this.videoConverter = videoConverter;
            this.parameterBusiness = parameterBusiness;
        }

        [Route("api/Item/AreasConhecimento")]
        [HttpGet]
        [ResponseType(typeof(BaseDto))]
        public HttpResponseMessage GetAllKnowledgeAreaActive()
        {
            try
            {
                var result = itemBusiness.LoadAllKnowledgeAreaActive();
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return null;
            }
        }

        [Route("api/Item/Disciplinas/AreaConhecimentoId")]
        [HttpGet]
        [ResponseType(typeof(DisciplineDto))]
        public HttpResponseMessage GetAlDisciplinebyknowledgearea(int areaConhecimentoId)
        {
            try
            {
                var result = itemBusiness.LoadDisciplineByKnowledgeArea(areaConhecimentoId);
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return null;
            }
        }

        [Route("api/Item/Matrizes/DisciplinaId")]
        [HttpGet]
        [ResponseType(typeof(MatrixDto))]
        public HttpResponseMessage GetEvaluationMatrixbyDiscipline(int disciplinaId)
        {
            try
            {
                if (disciplinaId <= 0)
                    return Request.CreateResponse(HttpStatusCode.BadRequest,
                        "O parametro disciplinaId é obrigatório e tem que ser maior que zero.");


                var lista = itemBusiness.LoadMatrixByDiscipline(disciplinaId);

                if (lista == null || !lista.Any())
                    return Request.CreateResponse(HttpStatusCode.NoContent, "Matriz de avaliação não encontrada.");

                // TODO return dto
                return Request.CreateResponse(HttpStatusCode.OK, lista);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError,
                    "Não foi possivel retornar a lista de matrizes dessa disciplina");
            }
        }

        [Route("api/Item/Competencias/MatrizId")]
        [HttpGet]
        [ResponseType(typeof(SkillDto))]
        public HttpResponseMessage GetSkillbymatriz(long matrizId)
        {
            try
            {
                if (matrizId <= 0)
                    return Request.CreateResponse(HttpStatusCode.BadRequest,
                        "O parametro matrizId é obrigatório e tem que ser maior que zero.");


                var lista = itemBusiness.LoadSkillByMatrix(matrizId);

                if (lista == null || !lista.Any())
                    return Request.CreateResponse(HttpStatusCode.NoContent, "Eixos não encontrados.");

                // TODO return dto 
                return Request.CreateResponse(HttpStatusCode.OK, lista);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError,
                    "Não foi possivel retornar a lista de eixos dessa matriz");
            }
        }

        [Route("api/Item/Habilidades/CompetenciaId")]
        [HttpGet]
        [ResponseType(typeof(AbilityDto))]
        public HttpResponseMessage GetAbilityBySkill(long competenciaId)
        {
            try
            {
                if (competenciaId <= 0)
                    return Request.CreateResponse(HttpStatusCode.BadRequest,
                        "O parametro competenciaId é obrigatório e tem que ser maior que zero.");

                var lista = itemBusiness.LoadAbilityBySkill(competenciaId);


                if (lista == null || !lista.Any())
                    return Request.CreateResponse(HttpStatusCode.NoContent, "Habilidades não encontrados.");

                // TODO return dto 
                return Request.CreateResponse(HttpStatusCode.OK, lista);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError,
                    "Não foi possivel retornar a lista de habilidades desse eixo");
            }
        }

        [Route("api/Item/Assuntos")]
        [HttpGet]
        [ResponseType(typeof(BaseDto))]
        public HttpResponseMessage GetAllSubjects()
        {
            try
            {
                var lista = itemBusiness.LoadAllSubjects();
                if (lista == null || !lista.Any())
                    return Request.CreateResponse(HttpStatusCode.NoContent, "Assuntos não encontrados.");

                return Request.CreateResponse(HttpStatusCode.OK, lista);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError,
                    "Não foi possivel retornar a lista de assuntos");
            }
        }

        [Route("api/Item/Assuntos/DisciplinaId")]
        [HttpGet]
        [ResponseType(typeof(BaseDto))]
        public HttpResponseMessage ObterAssuntosPorDisciplina(int disciplinaId)
        {
            try
            {
                var lista = itemBusiness.ObterAssuntosPorDisciplina(disciplinaId);
                if (lista == null || !lista.Any())
                    return Request.CreateResponse(HttpStatusCode.NoContent, "Assuntos não encontrados.");

                return Request.CreateResponse(HttpStatusCode.OK, lista);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError,
                    "Não foi possivel retornar a lista de assuntos");
            }
        }

        [Route("api/Item/SubAssuntos/AssuntoId")]
        [HttpGet]
        [ResponseType(typeof(BaseDto))]
        public HttpResponseMessage GetSubsubjectBySubject(int assuntoId)
        {
            try
            {
                if (assuntoId <= 0)
                    return Request.CreateResponse(HttpStatusCode.BadRequest,
                        "O parametro assuntoId é obrigatório e tem que ser maior que zero.");
                var lista = itemBusiness.LoadSubsubjectBySubject(assuntoId.ToString());
                if (lista == null || !lista.Any())
                    return Request.CreateResponse(HttpStatusCode.NoContent, "Assuntos não encontrados.");
                return Request.CreateResponse(HttpStatusCode.OK, lista);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError,
                    "Não foi possivel retornar a lista de assuntos");
            }
        }

        [Route("api/Item/Tipos")]
        [HttpGet]
        [ResponseType(typeof(BaseDto))]
        public HttpResponseMessage GetItemTypes()
        {
            try
            {
                var lista = itemBusiness.FindForTestType();

                if (lista == null || !lista.Any())
                    return Request.CreateResponse(HttpStatusCode.NoContent, "Itens não encontrados.");
                return Request.CreateResponse(HttpStatusCode.OK, lista);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError,
                    "Não foi possivel retornar a lista de tipos de itens.");
            }
        }

        [Route("api/Item/TiposGradeCurricular/MatrizId")]
        [HttpGet]
        [ResponseType(typeof(List<CurriculumGradeDto>))]
        public HttpResponseMessage GetCurriculumGradesByMatrix(int evaluationMatrixId)
        {
            try
            {
                if (evaluationMatrixId <= 0)
                    return Request.CreateResponse(HttpStatusCode.BadRequest,
                        "O parametro evaluationMatrixId é obrigatório e tem que ser maior que zero.");
                var lista = itemBusiness.LoadCurriculumGradesByMatrix(evaluationMatrixId);

                if (lista == null || !lista.Any())
                    return Request.CreateResponse(HttpStatusCode.NoContent, "Os anos da matriz não foram encontrados.");

                // TODO return dto
                return Request.CreateResponse(HttpStatusCode.OK, lista);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError,
                    "Não foi possivel retornar a lista de habilidades desse eixo");
            }
        }

        [Route("api/Item/Dificuldades")]
        [HttpGet]
        [ResponseType(typeof(List<ItemLevelDto>))]
        public HttpResponseMessage GetAllItemLevel()
        {
            try
            {
                var lista = itemBusiness.LoadAllItemLevel();
                if (lista == null || !lista.Any())
                    return Request.CreateResponse(HttpStatusCode.NoContent,
                        "As dificuldades sugeridas não foram encontrados.");

                return Request.CreateResponse(HttpStatusCode.OK, lista);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError,
                    "Não foi possivel retornar a lista de deficuldades sugeridas");
            }
        }

        [Route("api/Item/Salvar")]
        [HttpPost]
        [ResponseType(typeof(List<ItemApiResult>))]
        public HttpResponseMessage ItemSave([FromBody] List<ItemApiDto> items)
        {
            List<ItemApiResult> lista = new List<ItemApiResult>();
            try
            {
                if (items == null || !items.Any())
                {
                    var itemResult = new ItemApiResult
                    {
                        sucesso = false,
                        tipo = ValidateType.error.ToString(),
                        mensagem = "Estrutura do json informado é inválida."
                    };

                    lista.Add(itemResult);
                }
                else
                {
                    lista = itemBusiness.SaveApi(items);
                }
            }
            catch (Exception ex)
            {
                var itemResult = new ItemApiResult
                {
                    sucesso = false,
                    tipo = ValidateType.error.ToString(),
                    mensagem = "Erro ao salvar item(s). erro original: " + ex.Message
                };
                lista.Add(itemResult);

                LogFacade.SaveBasicError(ex.Message);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, itemResult);
            }

            return Request.CreateResponse(HttpStatusCode.OK, lista);
        }

        [Route("api/Item")]
        [HttpGet]
        [ResponseType(typeof(ItemConsultaApiPaginadoDto))]
        public HttpResponseMessage GetItem(int pagina, int qtdePorPagina, int areaConhecimentoId, long? matrizId = null)
        {
            try
            {
                var items = itemBusiness.GetApi(pagina, qtdePorPagina, areaConhecimentoId, matrizId);
                return Request.CreateResponse(HttpStatusCode.OK, items);
            }
            catch (Exception ex)
            {
                LogFacade.SaveBasicError(ex.Message);
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }

        [Route("api/Item/Arquivos")]
        [HttpGet]
        [ResponseType(typeof(ArquivosItemConsultaApiDto))]
        public HttpResponseMessage ObterArquivosItem(long itemId)
        {
            try
            {
                var arquivosItem = itemBusiness.ObterArquivosItemApi(itemId);
                return Request.CreateResponse(HttpStatusCode.OK, arquivosItem);
            }
            catch (Exception ex)
            {
                LogFacade.SaveBasicError(ex.Message);
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }

        [Route("api/Item/Arquivos/Upload")]
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
                    FileType = EnumFileType.File,
                    UsuId = null
                };
                
                entity = fileBusiness.Upload(upload);
            }
            catch (Exception ex)
            {
                entity.Validate.IsValid = false;
                entity.Validate.Type = ValidateType.error.ToString();
                entity.Validate.Message = $"Erro ao realizar o upload do arquivo.";
                LogFacade.SaveErrorSme(ex);
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

        [Route("api/Item/Arquivos/UploadVideo")]
        [HttpPost]
        [ResponseType(typeof(ResponseUploadArquivoVideoDto))]
        public async Task<HttpResponseMessage> UploadVideoAsync(Uploader file)
        {
            var entity = new EntityFile();
            var entityFileConvert = new EntityFile();
            try
            {
                var bytes= Convert.FromBase64String(file.InputStream);
                Stream stream = new MemoryStream(bytes);

                var sizeToConvertVideo = parameterBusiness.GetByKey(EnumParameterKey.SIZE_TO_CONVERT_VIDEO_FILE.GetDescription());
                if (file.ContentLength >= int.Parse(sizeToConvertVideo.Value))
                    entityFileConvert = await ConvertVideoAsync(stream, file.ContentType, file.FileName);

                var upload = new UploadModel
                {
                    ContentLength = file.ContentLength,
                    ContentType = file.ContentType,
                    InputStream = null,
                    Stream = stream,
                    FileName = file.FileName,
                    VirtualDirectory = ApplicationFacade.VirtualDirectorySme,
                    PhysicalDirectory = ApplicationFacade.PhysicalDirectorySme,
                    FileType = EnumFileType.Video,
                    UsuId = null
                };

                entity = fileBusiness.Upload(upload);

                if (!entityFileConvert.Validate.IsValid)
                    entity.Validate.Message += entityFileConvert.Validate.Message;
            }
            catch (Exception ex)
            {
                entity.Validate.IsValid = false;
                entity.Validate.Type = ValidateType.error.ToString();
                entity.Validate.Message = "Erro ao realizar o upload do arquivo de vídeo.";
                LogFacade.SaveError(ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK, new ResponseUploadArquivoVideoDto
            {
                Success = entity.Validate.IsValid,
                Type = entity.Validate.Type,
                Message = entity.Validate.Message,
                FileLink = entity.Path,
                IdFile = entity.Id,
                IdConvertedFile = entityFileConvert.Id
            });
        }

        [Route("api/Item/Arquivos/UploadAudio")]
        [HttpPost]
        [ResponseType(typeof(ResponseUploadArquivoAudioDto))]
        public HttpResponseMessage UploadAudio(Uploader file)
        {
            var entity = new EntityFile();
            try
            {
                var bytes = Convert.FromBase64String(file.InputStream);
                Stream stream = new MemoryStream(bytes);

                var upload = new UploadModel
                {
                    ContentLength = file.ContentLength,
                    ContentType = file.ContentType,
                    InputStream = null,
                    Stream = stream,
                    FileName = file.FileName,
                    VirtualDirectory = ApplicationFacade.VirtualDirectorySme,
                    PhysicalDirectory = ApplicationFacade.PhysicalDirectorySme,
                    FileType = EnumFileType.Audio,
                    UsuId = null
                };

                entity = fileBusiness.Upload(upload);
            }
            catch (Exception ex)
            {
                entity.Validate.IsValid = false;
                entity.Validate.Type = ValidateType.error.ToString();
                entity.Validate.Message = "Erro ao realizar o upload do arquivo de áudio.";
                LogFacade.SaveError(ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK, new ResponseUploadArquivoAudioDto
            {
                Success = entity.Validate.IsValid,
                Type = entity.Validate.Type,
                Message = entity.Validate.Message,
                FileLink = entity.Path,
                IdFile = entity.Id
            });
        }

        private async Task<EntityFile> ConvertVideoAsync(Stream inputStream, string contentType, string fileName)
        {
            var entity = new EntityFile();

            var convertedVideoDto = await videoConverter.Convert(inputStream, contentType, fileName, SessionFacade.UsuarioLogado.Usuario.usu_id);
            if (convertedVideoDto is null)
            {
                entity.Validate.IsValid = false;
                entity.Validate.Type = ValidateType.error.ToString();
                entity.Validate.Message =
                    "Não foi possível realizar a conversão do vídeo para um tamanho menor. O vídeo origianl será mantido.";
                return entity;
            }

            var uploadConvertedVideo = new UploadModel
            {
                ContentLength = (int)convertedVideoDto.Stream.Length,
                ContentType = VideoConvertedContentType,
                InputStream = null,
                Stream = convertedVideoDto.Stream,
                FileName = convertedVideoDto.FileName,
                VirtualDirectory = ApplicationFacade.VirtualDirectory,
                PhysicalDirectory = ApplicationFacade.PhysicalDirectory,
                FileType = EnumFileType.Video,
                UsuId = SessionFacade.UsuarioLogado.Usuario.usu_id
            };

            return fileBusiness.Upload(uploadConvertedVideo);
        }
    }
}