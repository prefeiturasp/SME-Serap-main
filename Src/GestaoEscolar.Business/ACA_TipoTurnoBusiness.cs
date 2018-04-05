using GestaoEscolar.Entities;
using GestaoEscolar.IBusiness;
using GestaoEscolar.IRepository;
using System.Collections.Generic;

namespace GestaoEscolar.Business
{
    public class ACA_TipoTurnoBusiness : IACA_TipoTurnoBusiness
	{
		private readonly IACA_TipoTurnoRepository tipoTurnoRepository;

		public ACA_TipoTurnoBusiness(IACA_TipoTurnoRepository tipoTurnoRepository)
		{
			this.tipoTurnoRepository = tipoTurnoRepository;
		}

		public IEnumerable<ACA_TipoTurno> Load(int esc_id)
		{
			return tipoTurnoRepository.Load(esc_id);
		}

        public ACA_TipoTurno Get(int id)
        {
            return tipoTurnoRepository.Get(id);
        }
	}
}
