using GestaoAvaliacao.API.App_Start;
using GestaoAvaliacao.API.Models;
using GestaoAvaliacao.Entities;
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
    [CustomAuthorizationAttribute]
	public class AdherenceController : ApiController
	{

		readonly IAdherenceBusiness adherenceBusiness;

		public AdherenceController(IAdherenceBusiness adherenceBusiness)
		{
			this.adherenceBusiness = adherenceBusiness;
		}

		[Route("api/adherence")]
		[HttpPost]
		[ResponseType(typeof(AdherenceResult))]
		public HttpResponseMessage Post([FromBody]AdherenceModel model)
		{
			Adherence entity;
			try
			{
				entity = adherenceBusiness.Select(long.Parse(this.UserData()), this.UserId(), model.idEntity, model.typeEntity, 
					model.typeSelection, this.PesId(), this.EntityId(), this.VisId(), model.ttn_id, model.year, model.parentId);

				var result = new AdherenceResult()
				{
					id = model.idEntity,
					success = entity.Validate.IsValid,
					type = entity.Validate.Type,
					message = entity.Validate.Message
				};

				return Request.CreateResponse(HttpStatusCode.OK, result);
			}
			catch (Exception ex)
			{
				LogFacade.SaveError(ex);
				var result = new AdherenceResult()
				{
					success = false,
					type = "error",
					message = "Erro ao salvar a adesão da prova.",
					id = model.idEntity
				};

				return Request.CreateResponse(HttpStatusCode.OK, result);
			}
		}

		[Route("api/adherence/List")]
		[HttpPost]
		[ResponseType(typeof(IEnumerable<AdherenceResult>))]
		public HttpResponseMessage Post([FromBody]AdherenceListModel model)
		{
			List<Adherence> entities;
			try
			{
				entities = adherenceBusiness.Select(long.Parse(this.UserData()), this.UserId(), model.entityList,
					model.typeSelection, this.PesId(), this.EntityId(), this.VisId(), model.ttn_id, model.year);

				var result = entities.Where(e => !e.Validate.IsValid).Select(e => new AdherenceResult()
				{
					id = e.EntityId,
					message = e.Validate.Message,
					success = e.Validate.IsValid,
					type = e.Validate.Type
				});

				return Request.CreateResponse(HttpStatusCode.OK, result);
			}
			catch (Exception ex)
			{
				LogFacade.SaveError(ex);
				var result = new AdherenceResult()
				{
					success = false,
					type = "error",
					message = "Erro ao salvar a adesão da prova.",
				};

				return Request.CreateResponse(HttpStatusCode.OK, result);
			}
		}
	}
}