using GestaoEscolar.Entities;
using System;
using System.Collections.Generic;
using CoreSSO = MSTech.CoreSSO.Entities;

namespace GestaoEscolar.IBusiness
{
    public interface ISYS_UnidadeAdministrativaBusiness
	{
		IEnumerable<SYS_UnidadeAdministrativa> LoadDRESimple(CoreSSO.SYS_Usuario user, CoreSSO.SYS_Grupo grupo);
        SYS_UnidadeAdministrativa GetByUad_Id(Guid uad_id);

    }
}
