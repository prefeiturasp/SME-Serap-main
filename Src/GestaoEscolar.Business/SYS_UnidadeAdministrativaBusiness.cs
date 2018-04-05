using GestaoAvaliacao.Util;
using GestaoEscolar.Entities;
using GestaoEscolar.IBusiness;
using GestaoEscolar.IRepository;
using MSTech.CoreSSO.BLL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using CoreSSO = MSTech.CoreSSO.Entities;

namespace GestaoEscolar.Business
{
    public class SYS_UnidadeAdministrativaBusiness : ISYS_UnidadeAdministrativaBusiness
	{
		private readonly ISYS_UnidadeAdministrativaRepository unidadeAdministrativaRepository;

		public SYS_UnidadeAdministrativaBusiness(ISYS_UnidadeAdministrativaRepository unidadeAdministrativaRepository)
		{
			this.unidadeAdministrativaRepository = unidadeAdministrativaRepository;
		}

		public IEnumerable<SYS_UnidadeAdministrativa> LoadDRESimple(CoreSSO.SYS_Usuario user, CoreSSO.SYS_Grupo grupo)
		{
			EnumSYS_Visao visao = (EnumSYS_Visao)Enum.Parse(typeof(EnumSYS_Visao), grupo.vis_id.ToString());
			DataTable dt = null;
			switch (visao)
			{
				case EnumSYS_Visao.Administracao:
					return unidadeAdministrativaRepository.LoadSimple(user.ent_id);
				case EnumSYS_Visao.Gestao:
					dt = SYS_UsuarioGrupoUABO.GetSelect(user.usu_id, grupo.gru_id);
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        return unidadeAdministrativaRepository.LoadSimple(user.ent_id, dt.AsEnumerable().Select(x => string.Concat("'", x.Field<Guid>("uad_id"), "'")));
                    }
                    else { break; }
                case EnumSYS_Visao.UnidadeAdministrativa:
					dt = SYS_UsuarioGrupoUABO.GetSelect(user.usu_id, grupo.gru_id);
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        return unidadeAdministrativaRepository.LoadSimpleCoordinator(user.ent_id, dt.AsEnumerable().Select(x => string.Concat("'", x.Field<Guid>("uad_id"), "'")));
                    }
                    else { break; }
				case EnumSYS_Visao.Individual:
					return unidadeAdministrativaRepository.LoadSimpleTeacher(user.ent_id, user.pes_id);
				default:
					break;
			}
			return null;
		}

        public SYS_UnidadeAdministrativa GetByUad_Id(Guid uad_id)
        {
            return unidadeAdministrativaRepository.GetByUad_Id(uad_id);
        }
    }
}
