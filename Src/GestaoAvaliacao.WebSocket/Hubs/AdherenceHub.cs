using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Entities.Enumerator;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.Util;
using GestaoAvaliacao.WebSocket;
using Microsoft.AspNet.SignalR;
using System;
using System.Linq;
using System.Security.Claims;

namespace GestaoAvaliacao.WebSockets.Hubs
{
	[CustomAuthorization]
	public class AdherenceHub : Hub
	{
		#region Properties
		public ClaimsPrincipal Principal
		{
			get
			{
				return Context.User as ClaimsPrincipal;
			}
		}
		public Guid UserId
		{
			get
			{
				return new Guid(Principal.Claims.First(c => c.Type.Equals(ClaimTypes.Name)).Value);
			}
		}

		public Guid pes_id
		{
			get
			{
				return new Guid(Principal.Claims.First(c => c.Type.Equals(ClaimTypes.NameIdentifier)).Value);
			}
		}

		public EnumSYS_Visao vis_id
		{
			get
			{
				return (EnumSYS_Visao)byte.Parse(Principal.Claims.First(c => c.Type.Equals(ClaimTypes.Role)).Value);
			}
		}

		public Guid ent_id
		{
			get
			{
				return new Guid(Principal.Claims.First(c => c.Type.Equals(ClaimTypes.System)).Value);
			}
		}

		public string userData
		{
			get
			{
				return Principal.Claims.First(c => c.Type.Equals(ClaimTypes.UserData)).Value;
			}
		}

		private readonly IAdherenceBusiness adherenceBusiness;

		public AdherenceHub(IAdherenceBusiness adherenceBusiness)
		{
			this.adherenceBusiness = adherenceBusiness;
		}

		#endregion

		public void AdherenceSave(long idEntity, EnumAdherenceEntity typeEntity, EnumAdherenceSelection typeSelection, int ttn_id, int year)
		{
			Adherence entity;
			try
			{
				entity = adherenceBusiness.Select(long.Parse(userData), UserId, idEntity, typeEntity, typeSelection, pes_id, ent_id, vis_id, ttn_id, year);

			}
			catch (Exception)
			{
				entity = new Adherence();
				entity.Validate.IsValid = false;
				entity.Validate.Type = ValidateType.error.ToString();
				entity.Validate.Message = string.Format("Erro ao {0} a adesão da prova.", entity.Id > 0 ? "alterar" : "salvar");
			}
			Clients.Caller.EndAdherenceSave(new { id = idEntity, success = entity.Validate.IsValid, type = entity.Validate.Type, message = entity.Validate.Message });
		}
	}
}