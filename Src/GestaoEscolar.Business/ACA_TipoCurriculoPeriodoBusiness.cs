using GestaoAvaliacao.Util;
using GestaoEscolar.Entities;
using GestaoEscolar.Entities.DTO;
using GestaoEscolar.IBusiness;
using GestaoEscolar.IRepository;
using System;
using System.Collections.Generic;
using System.Data;

namespace GestaoEscolar.Business
{
    public class ACA_TipoCurriculoPeriodoBusiness : IACA_TipoCurriculoPeriodoBusiness
	{
        readonly IACA_TipoCurriculoPeriodoRepository tipoCurriculoPeriodoRepository;

		public ACA_TipoCurriculoPeriodoBusiness(IACA_TipoCurriculoPeriodoRepository tipoCurriculoPeriodoRepository)
		{
			this.tipoCurriculoPeriodoRepository = tipoCurriculoPeriodoRepository;
		}

		public IEnumerable<ACA_TipoCurriculoPeriodo> GetSimple(int esc_id)
		{
			return tipoCurriculoPeriodoRepository.GetSimple(esc_id);
		}

		public ACA_TipoCurriculoPeriodo Get(int tcp_id)
		{
			return tipoCurriculoPeriodoRepository.Get(tcp_id);
		}

		public IEnumerable<ACA_TipoCurriculoPeriodo> Load()
		{
			return tipoCurriculoPeriodoRepository.Load();
		}

		public IEnumerable<ACA_TipoCurriculoPeriodo> GetAllTypeCurriculumGrades()
		{
			return tipoCurriculoPeriodoRepository.GetAllTypeCurriculumGrades();
		}

		public IEnumerable<ACA_TipoCurriculoPeriodo> LoadByLevelEducationModality(int tne_id, int tme_id)
		{
			return tipoCurriculoPeriodoRepository.LoadByLevelEducationModality(tne_id, tme_id);
		}

		public string GetDescription(int tcp_id, int tne_id, int tme_id, int tcp_ordem)
		{
			return tipoCurriculoPeriodoRepository.GetDescription(tcp_id, tne_id, tme_id, tcp_ordem);
		}

		public int GetId(int tne_id, int tme_id, int tcp_ordem)
		{
			return tipoCurriculoPeriodoRepository.GetId(tne_id, tme_id, tcp_ordem);
		}

		public IEnumerable<TUR_TurmaTipoCurriculoPeriodoDTO> LoadWithNivelEnsino(MSTech.CoreSSO.Entities.SYS_Usuario user, MSTech.CoreSSO.Entities.SYS_Grupo group)
		{
			var vision = (EnumSYS_Visao)Enum.Parse(typeof(EnumSYS_Visao), group.vis_id.ToString());
			DataTable dt = MSTech.CoreSSO.BLL.SYS_UsuarioGrupoUABO.GetSelect(user.usu_id, group.gru_id);

			var uads = dt.AsEnumerable().Select(x => string.Concat("'", x.Field<Guid>("uad_id"), "'"));

			switch (vision)
			{
				case EnumSYS_Visao.Administracao:
					return tipoCurriculoPeriodoRepository.LoadWithNivelEnsino();
				case EnumSYS_Visao.Gestao:
					return tipoCurriculoPeriodoRepository.LoadWithNivelEnsino(user.ent_id, dre_id: uads);
				case EnumSYS_Visao.UnidadeAdministrativa:
					return tipoCurriculoPeriodoRepository.LoadWithNivelEnsino(user.ent_id, esc_id: uads);
				case EnumSYS_Visao.Individual:
					return tipoCurriculoPeriodoRepository.LoadWithNivelEnsino(user.ent_id, pes_id: user.pes_id);
			}

			return null;
		}
		public IEnumerable<TUR_TurmaTipoCurriculoPeriodoDTO> LoadWithNivelEnsino()
		{
			return tipoCurriculoPeriodoRepository.LoadWithNivelEnsino();
		}
	}
}
