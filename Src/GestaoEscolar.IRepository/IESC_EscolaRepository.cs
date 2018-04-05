using GestaoEscolar.Entities;
using GestaoEscolar.Entities.Projections;
using System;
using System.Collections.Generic;

namespace GestaoEscolar.IRepository
{
    public interface IESC_EscolaRepository
	{
		IEnumerable<ESC_Escola> LoadSimple(Guid ent_id, Guid uad_id, IEnumerable<string> esc_id = null);
		IEnumerable<ESC_Escola> LoadSimpleTeacher(Guid ent_id, Guid pes_id, Guid uad_id);
		int GetTotalSchool(Guid ent_id, IEnumerable<string> uad_id = null);
		ESC_Escola Get(int esc_id);
        ESC_Escola GetWithAdministrativeUnity(Guid ent_id, long esc_id);

        /// <summary>
        /// Busca o nome da DRE e da escola
        /// </summary>
        /// <param name="esc_id">Id da escola</param>
        /// <returns>Projection com o nome da DRE e da escola</returns>
        SchoolAndDRENamesProjection GetSchoolAndDRENames(int esc_id);
    }
}
