using GestaoAvaliacao.API.Middleware;
using GestaoAvaliacao.API.Models;
using GestaoAvaliacao.Dtos.ItemApi;
using GestaoAvaliacao.Entities;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.WebProject.Facade;
using GestaoEscolar.IBusiness;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;


namespace GestaoAvaliacao.API.Controllers
{
    [AutAttribute]
    public class ItemController : ApiController
    {
        private readonly IItemBusiness itemBusiness;
        private readonly IACA_TipoNivelEnsinoBusiness levelEducationBusiness;
        private readonly IFileBusiness fileBusiness;

        public ItemController(IItemBusiness itemBusiness, IFileBusiness fileBusiness)
        {
            this.itemBusiness = itemBusiness;
            this.fileBusiness = fileBusiness;
        }


        [Route("api/Item/AreasConhecimento")]
        [HttpGet]
        [ResponseType(typeof(BaseDto))]
        public async Task<HttpResponseMessage> GetAllKnowledgeAreaActive()
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
        [ResponseType(typeof(BaseDto))]
        public async Task<HttpResponseMessage> GetAlDisciplinebyknowledgearea(int areaConhecimentoId)
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
        [ResponseType(typeof(BaseDto))]
        public async Task<HttpResponseMessage> GetEvaluationMatrixbyDiscipline(int disciplinaId)
        {
            try
            {
                if (disciplinaId <= 0)
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "O parametro disciplinaId é obrigatório e tem que ser maior que zero.");


                var lista = itemBusiness.LoadMatrixByDiscipline(disciplinaId);

                if (lista == null || !lista.Any())
                    return Request.CreateResponse(HttpStatusCode.NoContent, "Matriz de avaliação não encontrada.");

                // TODO return dto



                return Request.CreateResponse(HttpStatusCode.OK, lista);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "Não foi possivel retornar a lista de matrizes dessa disciplina");
            }
        }


        [Route("api/Item/Eixos/MatrizId")]
        [HttpGet]
        [ResponseType(typeof(SkillDto))]
        public async Task<HttpResponseMessage> GetSkillbymatriz(long matrizId)
        {
            try
            {
                if (matrizId <= 0)
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "O parametro matrizId é obrigatório e tem que ser maior que zero.");


                var lista = itemBusiness.LoadSkillByMatrix(matrizId);

                if (lista == null || !lista.Any())
                    return Request.CreateResponse(HttpStatusCode.NoContent, "Eixos não encontrados.");
                // TODO return dto 
                return Request.CreateResponse(HttpStatusCode.OK, lista);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "Não foi possivel retornar a lista de eixos dessa matriz");
            }
        }


        [Route("api/Item/Habilidade/EixoId")]
        [HttpGet]
        [ResponseType(typeof(AbilityDto))]
        public async Task<HttpResponseMessage> GetAbilityBySkill(long eixoId)
        {
            try
            {
                if (eixoId <= 0)
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "O parametro eixoId é obrigatório e tem que ser maior que zero.");

                var lista = itemBusiness.LoadAbilityBySkill(eixoId);


                if (lista == null || !lista.Any())
                    return Request.CreateResponse(HttpStatusCode.NoContent, "Habilidades não encontrados.");
                // TODO return dto 
                return Request.CreateResponse(HttpStatusCode.OK, lista);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "Não foi possivel retornar a lista de habilidades desse eixo");
            }
        }

        [Route("api/Item/Assuntos")]
        [HttpGet]
        [ResponseType(typeof(BaseDto))]
        public async Task<HttpResponseMessage> GetAllSubjects()
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
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "Não foi possivel retornar a lista de assuntos");
            }
        }


        [Route("api/Item/Assuntos/SubAssuntos/AssuntoId")]
        [HttpGet]
        [ResponseType(typeof(BaseDto))]
        public async Task<HttpResponseMessage> GetSubsubjectBySubject(int assuntoId)
        {
            try
            {
                if (assuntoId <= 0)
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "O parametro assuntoId é obrigatório e tem que ser maior que zero.");
                var lista = itemBusiness.LoadSubsubjectBySubject(assuntoId.ToString());
                if (lista == null || !lista.Any())
                    return Request.CreateResponse(HttpStatusCode.NoContent, "Assuntos não encontrados.");
                return Request.CreateResponse(HttpStatusCode.OK, lista);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "Não foi possivel retornar a lista de assuntos");
            }
        }

        [Route("api/Item/Tipos")]
        [HttpGet]
        [ResponseType(typeof(BaseDto))]
        public async Task<HttpResponseMessage> GetItemTypes()
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
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "Não foi possivel retornar a lista de tipos de itens.");
            }
        }

        [Route("api/Item/CurriculumGrades/EvaluationMatrixId")]
        [HttpGet]
        [ResponseType(typeof(List<CurriculumGradeDto>))]
        public async Task<HttpResponseMessage> GetCurriculumGradesByMatrix(int evaluationMatrixId)
        {
            try
            {
                if (evaluationMatrixId <= 0)
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "O parametro evaluationMatrixId é obrigatório e tem que ser maior que zero.");
                var lista = itemBusiness.LoadCurriculumGradesByMatrix(evaluationMatrixId);

                if (lista == null || !lista.Any())
                    return Request.CreateResponse(HttpStatusCode.NoContent, "Os anos da matriz não foram encontrados.");
                // TODO return dto
                return Request.CreateResponse(HttpStatusCode.OK, lista);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "Não foi possivel retornar a lista de habilidades desse eixo");
            }
        }


        [Route("api/Item/Save")]
        [HttpPost]
        [ResponseType(typeof(ItemApiResult))]
        public HttpResponseMessage ItemSave([FromBody] ItemModel model)
        {
            ItemApiResult itemResult = new ItemApiResult();
            try
            {
                var files = new List<File>();
                if (model.Pictures != null && model.Pictures.Count > 0)
                {
                    foreach (var picture in model.Pictures)
                    {
                        switch (picture.Type)
                        {
                            case PictureType.BaseText:
                                if (model.BaseText.Description.Contains(picture.Tag))
                                {
                                    string tabImg = UploadPictureTagImg(EnumFileType.BaseText, files, picture);
                                    model.BaseText.Description = model.BaseText.Description.Replace(picture.Tag, tabImg);
                                }
                                break;
                            case PictureType.Statement:
                                if (model.Statement.Contains(picture.Tag))
                                {
                                    string tabImg = UploadPictureTagImg(EnumFileType.Statement, files, picture);
                                    model.Statement = model.Statement.Replace(picture.Tag, tabImg);
                                }
                                break;

                            case PictureType.Alternative:
                                foreach (var alternative in model.Alternatives)
                                {
                                    if (alternative.Description.Contains(picture.Tag))
                                    {
                                        string tabImg = UploadPictureTagImg(EnumFileType.Alternative, files, picture);
                                        alternative.Description = alternative.Description.Replace(picture.Tag, tabImg);
                                    }
                                }
                                break;
                            case PictureType.Justificative:
                                foreach (var alternative in model.Alternatives)
                                {
                                    if (alternative.Justificative.Contains(picture.Tag))
                                    {
                                        string tabImg = UploadPictureTagImg(EnumFileType.Justificative, files, picture);
                                        alternative.Justificative = alternative.Justificative.Replace(picture.Tag, tabImg);
                                    }
                                }
                                break;
                        }
                    }
                }

                Item item = new Item()
                {
                    ItemCodeVersion = model.ItemCodeVersion,
                    Statement = model.Statement,
                    descriptorSentence = model.DescriptorSentence,
                    proficiency = model.Proficiency,
                    EvaluationMatrix_Id = model.EvaluationMatrix_Id,
                    Keywords = model.Keywords,
                    Tips = model.Tips,
                    TRICasualSetting = model.TRICasualSetting,
                    TRIDifficulty = model.TRIDifficulty,
                    TRIDiscrimination = model.TRIDiscrimination,
                    BaseText = new BaseText()
                    {
                        Description = model.BaseText.Description,
                        Source = model.BaseText.Description
                    },
                    ItemSituation_Id = model.ItemSituation_Id,
                    ItemType_Id = model.ItemType_Id,
                    ItemLevel_Id = model.ItemLevel_Id,
                    ItemCode = model.ItemCode,
                    ItemVersion = model.ItemVersion,
                    ItemCurriculumGrades = new List<ItemCurriculumGrade>()
                    {
                        new ItemCurriculumGrade() {
                            TypeCurriculumGradeId = model.TypeCurriculumGradeId
                        }
                    },
                    ItemSkills = model.ItemSkills.Select(t => new ItemSkill()
                    {
                        Skill_Id = t
                    }).ToList(),
                    Alternatives = model.Alternatives.Select(t => new Alternative()
                    {
                        Description = t.Description,
                        Correct = t.Correct,
                        Order = t.Order,
                        Justificative = t.Justificative,
                        Numeration = t.Numeration
                    }).ToList(),
                    IsRestrict = model.IsRestrict,
                    ItemNarrated = model.ItemNarrated,
                    StudentStatement = model.StudentStatement,
                    NarrationStudentStatement = model.NarrationStudentStatement,
                    NarrationAlternatives = model.NarrationAlternatives,
                    KnowledgeArea_Id = model.KnowledgeArea_Id,
                    SubSubject_Id = model.SubSubject_Id
                };

                var entity = itemBusiness.Save(0, item, files);

                itemResult.success = entity.Validate.IsValid;
                itemResult.type = entity.Validate.Type.ToString();
                itemResult.message = entity.Validate.Message;
                itemResult.item_id = entity.Id;
            }
            catch (Exception ex)
            {
                itemResult.success = false;
                itemResult.type = ValidateType.error.ToString();
                itemResult.message = "Erro ao salvar item.";

                LogFacade.SaveError(ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK, itemResult);
        }

        private string UploadPictureTagImg(EnumFileType type, List<File> files, PictureModel picture)
        {
            UploadModel upload = new UploadModel
            {
                ContentLength = picture.ContentLength,
                ContentType = picture.ContentType,
                InputStream = picture.InputStream,
                Stream = null,
                FileName = picture.FileName,
                VirtualDirectory = ApplicationFacade.VirtualDirectory,
                PhysicalDirectory = ApplicationFacade.PhysicalDirectory,
                FileType = type,
                UsuId = this.UserId()
            };

            var file = fileBusiness.Upload(upload);
            files.Add(file);


            var tabImg = $"<img src='{file.Path}' id='{file.Id}'>";
            return tabImg;
        }
    }
}