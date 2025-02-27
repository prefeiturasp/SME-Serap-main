using GestaoEscolar.Entities;
using System;
using System.Collections.Generic;

namespace GestaoEscolar.IRepository
{
    public interface ISYS_UnidadeAdministrativaRepository
	{
		IEnumerable<SYS_UnidadeAdministrativa> LoadSimple(Guid ent_id, IEnumerable<string> uad_id = null);
		IEnumerable<SYS_UnidadeAdministrativa> LoadSimpleTeacher(Guid ent_id, Guid pes_id);
		IEnumerable<SYS_UnidadeAdministrativa> LoadSimpleCoordinator(Guid ent_id, IEnumerable<string> uad_id);
        SYS_UnidadeAdministrativa GetByUad_Id(Guid uad_id);
        SYS_UnidadeAdministrativa GetByUad_Codigo(string uad_codigo);
    }
}
