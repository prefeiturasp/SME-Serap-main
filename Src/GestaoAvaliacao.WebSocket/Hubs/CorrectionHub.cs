using GestaoAvaliacao.Entities;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.MongoEntities;
using GestaoAvaliacao.Util;
using GestaoAvaliacao.WebSocket;
using Microsoft.AspNet.SignalR;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GestaoAvaliacao.WebSockets.Hubs
{
	[CustomAuthorization]
	public class CorrectionHub : Hub
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

		public long test_id
		{
			get
			{
				return long.Parse(userData.Split('_')[0]);
			}
		}

		public long tur_id
		{
			get
			{
				return long.Parse(userData.Split('_')[1]);
			}
		}

		private readonly ICorrectionBusiness correctionBusiness;

		public CorrectionHub(ICorrectionBusiness correctionBusiness)
		{
			this.correctionBusiness = correctionBusiness;
		}

		#endregion

		public async Task<object> CorrectionSave(long alu_id, long alternative_id, long item_id, bool n, bool r)
		{
			StudentCorrection entity;

			try
			{
				entity = await correctionBusiness.SaveCorrection(alu_id, alternative_id, item_id, n, r, test_id, tur_id, ent_id, UserId, pes_id, vis_id);
			}
			catch (Exception)
			{
				entity = new StudentCorrection();
				entity.Validate.IsValid = false;
				entity.Validate.Type = ValidateType.error.ToString();
				entity.Validate.Message = "Erro ao salvar nota do aluno.";
			}

			return new { success = entity.Validate.IsValid, type = entity.Validate.Type, message = entity.Validate.Message, alu_id = alu_id, item_id = item_id };
		}

		public async Task<object> AbsenceSave(long alu_id, long absenceReasonId)
		{
			StudentTestAbsenceReason entity = new StudentTestAbsenceReason();
			try
			{
				entity = await correctionBusiness.SaveAbsenceReason(
					new StudentTestAbsenceReason() { Test_Id = this.test_id, tur_id = tur_id, alu_id = alu_id, AbsenceReason_Id = absenceReasonId },
					UserId, pes_id, vis_id, ent_id);

			}
			catch (Exception)
			{
				entity = new StudentTestAbsenceReason();
				entity.Validate.IsValid = false;
				entity.Validate.Type = ValidateType.error.ToString();
				entity.Validate.Message = string.Format("Erro ao {0} motivo de ausência do aluno.", entity.Id > 0 ? "alterar" : "salvar");
			}

			return new { success = entity.Validate.IsValid, type = entity.Validate.Type, message = entity.Validate.Message, alu_id = alu_id };
		}
	}
}