using GestaoEscolar.Entities;
using System.Collections.Generic;

namespace GestaoEscolar.IBusiness
{
    public interface IACA_TipoTurnoBusiness
	{
		IEnumerable<ACA_TipoTurno> Load(int esc_id);
        ACA_TipoTurno Get(int id);
	}
}
