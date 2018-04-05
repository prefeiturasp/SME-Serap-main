using GestaoEscolar.Entities;
using GestaoEscolar.Entities.Projections;
using System;
using System.Collections.Generic;
using CoreSSO = MSTech.CoreSSO.Entities;

namespace GestaoEscolar.IBusiness
{
    public interface IESC_EscolaBusiness
	{
		IEnumerable<ESC_Escola> LoadSimple(CoreSSO.SYS_Usuario user, CoreSSO.SYS_Grupo grupo, Guid uad_id);
		int GetTotalSchool(CoreSSO.SYS_Usuario user, CoreSSO.SYS_Grupo grupo);
		ESC_Escola Get(int esc_id);

        /// <summary>
        /// Busca o nome da DRE e da escola
        /// </summary>
        /// <param name="esc_id">Id da escola</param>
        /// <returns>Projection com o nome da DRE e da escola</returns>
        SchoolAndDRENamesProjection GetSchoolAndDRENames(int esc_id);
        ESC_Escola GetWithAdministrativeUnity(Guid ent_id, long esc_id);
		IEnumerable<ESC_Escola> LoadSimpleTeacher(Guid ent_id, Guid pes_id, Guid uad_id);

    }
}
