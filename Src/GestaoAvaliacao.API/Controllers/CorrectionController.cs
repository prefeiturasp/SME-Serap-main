using GestaoAvaliacao.API.App_Start;
using GestaoAvaliacao.API.Models;
using GestaoAvaliacao.Entities;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.MongoEntities;
using GestaoAvaliacao.Util;
using GestaoAvaliacao.WebProject.Facade;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace GestaoAvaliacao.API.Controllers
{
    [CustomAuthorizationAttribute]
    public class CorrectionController : ApiController
    {
        #region Propriedades
        public long test_id
        {
            get
            {
                return long.Parse(this.UserData().Split('_')[0]);
            }
        }

        public long tur_id
        {
            get
            {
                return long.Parse(this.UserData().Split('_')[1]);
            }
        }
        #endregion

        #region Dependences
        private readonly ICorrectionBusiness correctionBusiness;

        public CorrectionController(ICorrectionBusiness correctionBusiness)
        {
            this.correctionBusiness = correctionBusiness;  
        }
        #endregion

        [Route("api/Correction")]
        [HttpPost]
        [ResponseType(typeof(CorrectionResult))]
        public async Task<HttpResponseMessage> Post([FromBody] CorrectionModel model)
        {
            StudentCorrection entity;

            try
            {
                entity = await correctionBusiness.SaveCorrection(model.alu_id, model.alternative_id, model.item_id, model.n, model.r, test_id, tur_id, this.EntityId(),
                    this.UserId(), this.PesId(), this.VisId(), model.manual);

                var result = new CorrectionResult()
                {
                    success = entity.Validate.IsValid,
                    type = entity.Validate.Type,
                    message = entity.Validate.Message,
                    alu_id = model.alu_id,
                    item_id = model.item_id
                };

                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                var result = new CorrectionResult()
                {
                    success = false,
                    type = "alert",
                    message = "Erro ao salvar nota do aluno.",
                    alu_id = model.alu_id,
                    item_id = model.item_id
                };

                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
        }
        [Route("api/Correction/Absence")]
        [HttpPost]
        [ResponseType(typeof(CorrectionResult))]
        public async Task<HttpResponseMessage> Absence([FromBody] AbsenceModel model)
        {
            StudentTestAbsenceReason entity = new StudentTestAbsenceReason();
            try
            {
                entity = await correctionBusiness.SaveAbsenceReason(
                    new StudentTestAbsenceReason() { Test_Id = this.test_id, tur_id = tur_id, alu_id = model.alu_id, AbsenceReason_Id = model.absenceReasonId },
                    this.UserId(), this.PesId(), this.VisId(), this.EntityId());

                var result = new CorrectionResult()
                {
                    success = entity.Validate.IsValid,
                    type = entity.Validate.Type,
                    message = entity.Validate.Message,
                    alu_id = model.alu_id
                };

                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                var result = new CorrectionResult()
                {
                    success = false,
                    type = "alert",
                    message = "Erro ao salvar motivo de ausência do aluno.",
                    alu_id = model.alu_id
                };

                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
        }
    }
}