using GestaoAvaliacao.Util;
using GestaoEscolar.Entities;
using GestaoEscolar.Entities.Projections;
using GestaoEscolar.IBusiness;
using GestaoEscolar.IRepository;
using MSTech.CoreSSO.BLL;
using System;
using System.Collections.Generic;
using System.Data;
using CoreSSO = MSTech.CoreSSO.Entities;

namespace GestaoEscolar.Business
{
    public class ESC_EscolaBusiness : IESC_EscolaBusiness
	{
		private readonly IESC_EscolaRepository escolaRepository;

		public ESC_EscolaBusiness(IESC_EscolaRepository escolaRepository)
		{
			this.escolaRepository = escolaRepository;
		}


		#region Read

		public IEnumerable<ESC_Escola> LoadSimple(CoreSSO.SYS_Usuario user, CoreSSO.SYS_Grupo grupo, Guid uad_id)
		{
			EnumSYS_Visao visao = (EnumSYS_Visao)Enum.Parse(typeof(EnumSYS_Visao), grupo.vis_id.ToString());
			switch (visao)
			{
				case EnumSYS_Visao.Administracao:
				case EnumSYS_Visao.Gestao:
					return escolaRepository.LoadSimple(user.ent_id, uad_id);
				case EnumSYS_Visao.UnidadeAdministrativa:
					DataTable dt = SYS_UsuarioGrupoUABO.GetSelect(user.usu_id, grupo.gru_id);
					return escolaRepository.LoadSimple(user.ent_id, uad_id, dt.AsEnumerable().Select(x => string.Concat("'", x.Field<Guid>("uad_id"), "'")));
				case EnumSYS_Visao.Individual:
					return LoadSimpleTeacher(user.ent_id, user.pes_id, uad_id);
				default:
					break;
			}

			return null;
		}
		
		public IEnumerable<ESC_Escola> LoadSimpleTeacher(Guid ent_id, Guid pes_id, Guid uad_id)
		{
			return escolaRepository.LoadSimpleTeacher(ent_id, pes_id, uad_id);
		}

		public int GetTotalSchool(CoreSSO.SYS_Usuario user, CoreSSO.SYS_Grupo grupo)
		{
			EnumSYS_Visao visao = (EnumSYS_Visao)Enum.Parse(typeof(EnumSYS_Visao), grupo.vis_id.ToString());
			switch (visao)
			{
				case EnumSYS_Visao.Administracao:
					return escolaRepository.GetTotalSchool(user.ent_id);
				case EnumSYS_Visao.Gestao:
					DataTable dt = MSTech.CoreSSO.BLL.SYS_UsuarioGrupoUABO.GetSelect(user.usu_id, grupo.gru_id);

					var uads = dt.AsEnumerable().Select(x => string.Concat("'", x.Field<Guid>("uad_id"), "'"));

					return escolaRepository.GetTotalSchool(user.ent_id, uad_id: uads);
				default:
					break;
			}

			return 0;
		}

		public ESC_Escola Get(int esc_id)
		{
			return escolaRepository.Get(esc_id);
		}

        public ESC_Escola GetWithAdministrativeUnity(Guid ent_id, long esc_id)
        {
            return escolaRepository.GetWithAdministrativeUnity(ent_id, esc_id);
        }

        /// <summary>
        /// Busca o nome da DRE e da escola
        /// </summary>
        /// <param name="esc_id">Id da escola</param>
        /// <returns>Projection com o nome da DRE e da escola</returns>
        public SchoolAndDRENamesProjection GetSchoolAndDRENames(int esc_id)
        {
            return escolaRepository.GetSchoolAndDRENames(esc_id);
        }

		public IEnumerable<string> LoadAllSchoolCodesActive()
		{
			return escolaRepository.LoadAllSchoolCodesActive();
		}

        #endregion

    }
}
