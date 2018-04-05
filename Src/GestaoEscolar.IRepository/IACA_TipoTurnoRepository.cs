using GestaoEscolar.Entities;
using System.Collections.Generic;

namespace GestaoEscolar.IRepository
{
    public interface IACA_TipoTurnoRepository
	{
		IEnumerable<ACA_TipoTurno> Load(int esc_id);
        ACA_TipoTurno Get(int id);
	}
}
