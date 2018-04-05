using GestaoAvaliacao.Util;
using GestaoEscolar.Entities;
using GestaoEscolar.Entities.DTO;
using GestaoEscolar.IBusiness;
using GestaoEscolar.IRepository;
using System;
using System.Collections.Generic;
using System.Data;
using CoreSSO = MSTech.CoreSSO.Entities;

namespace GestaoEscolar.Business
{
    public class TUR_TurmaBusiness : ITUR_TurmaBusiness
	{
		private readonly ITUR_TurmaRepository turmaRepository;

		public TUR_TurmaBusiness(ITUR_TurmaRepository turmaRepository)
		{
			this.turmaRepository = turmaRepository;
		}


		#region Read
		public IEnumerable<TUR_Turma> Load(int esc_id, int ttn_id, Guid ent_id, Guid pes_id, EnumSYS_Visao vis_id)
		{
			if (vis_id == EnumSYS_Visao.Individual)
				return turmaRepository.Load(esc_id, ttn_id, pes_id, ent_id);
			else
				return turmaRepository.Load(esc_id, ttn_id);
		}

		public int GetTotalSection(CoreSSO.SYS_Usuario user, CoreSSO.SYS_Grupo grupo)
		{
			EnumSYS_Visao visao = (EnumSYS_Visao)Enum.Parse(typeof(EnumSYS_Visao), grupo.vis_id.ToString());
			switch (visao)
			{
				case EnumSYS_Visao.Administracao:
					return turmaRepository.GetTotalSection(user.ent_id);
				case EnumSYS_Visao.Gestao:
					DataTable dt = MSTech.CoreSSO.BLL.SYS_UsuarioGrupoUABO.GetSelect(user.usu_id, grupo.gru_id);

					var uads = dt.AsEnumerable().Select(x => string.Concat("'", x.Field<Guid>("uad_id"), "'"));

					return turmaRepository.GetTotalSection(user.ent_id, uad_id: uads);
				default:
					break;
			}

			return 0;
		}

		public TUR_Turma Get(long id)
		{
			return turmaRepository.Get(id);
		}


		public TUR_Turma GetWithTurno(long tur_id)
		{
			return turmaRepository.GetWithTurno(tur_id);
		}

        public TUR_TurmaDTO GetWithTurnoAndModality(long tur_id)
        {
            return turmaRepository.GetWithTurnoAndModality(tur_id);
        }

        public bool ValidateTeacherSection(long tur_id, Guid pes_id)
        {
            return turmaRepository.ValidateTeacherSection(tur_id, pes_id);
        }

		public IEnumerable<TUR_Turma> LoadByGrade(int esc_id, int ttn_id, Guid ent_id, Guid pes_id, EnumSYS_Visao vis_id, IEnumerable<int> years)
		{
			if (vis_id == EnumSYS_Visao.Individual)
				return turmaRepository.LoadByGrade(esc_id, ttn_id, years, pes_id, ent_id);
			else
				return turmaRepository.LoadByGrade(esc_id, ttn_id, years);
		}
		#endregion
	}
}
