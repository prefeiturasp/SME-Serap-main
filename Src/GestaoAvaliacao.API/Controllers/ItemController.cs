using GestaoAvaliacao.API.Middleware;
using GestaoAvaliacao.Dtos.ItemApi;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.Util;
using GestaoAvaliacao.WebProject.Facade;
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

        public ItemController(IItemBusiness itemBusiness)
        {
            this.itemBusiness = itemBusiness;
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

        [Route("api/Item/Competencias/MatrizId")]
        [HttpGet]
        [ResponseType(typeof(SkillDto))]
        public HttpResponseMessage GetSkillbymatriz(long matrizId)
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

        [Route("api/Item/Habilidades/CompetenciaId")]
        [HttpGet]
        [ResponseType(typeof(AbilityDto))]
        public HttpResponseMessage GetAbilityBySkill(long competenciaId)
        {
            try
            {
                if (competenciaId <= 0)
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "O parametro competenciaId é obrigatório e tem que ser maior que zero.");

                var lista = itemBusiness.LoadAbilityBySkill(competenciaId);


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
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "Não foi possivel retornar a lista de assuntos");
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
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "Não foi possivel retornar a lista de tipos de itens.");
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

        [Route("api/Item/Dificuldade")]
        [HttpGet]
        [ResponseType(typeof(List<ItemLevelDto>))]
        public HttpResponseMessage GetAllItemLevel()
        {
            try
            {
                var lista = itemBusiness.LoadAllItemLevel();
                if (lista == null || !lista.Any())
                    return Request.CreateResponse(HttpStatusCode.NoContent, "As dificuldades sugeridas não foram encontrados.");

                return Request.CreateResponse(HttpStatusCode.OK, lista);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "Não foi possivel retornar a lista de deficuldades sugeridas");
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
    }
}