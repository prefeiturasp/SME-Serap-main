using GestaoAvaliacao.Entities;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.IRepository;
using GestaoAvaliacao.Util;
using GestaoEscolar.Entities;
using GestaoEscolar.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GestaoAvaliacao.Business
{
	public class AdministrativeUnitTypeBusiness : IAdministrativeUnitTypeBusiness
    {
		private readonly IAdministrativeUnitTypeRepository administrativeUnitTypeRepository;

		public AdministrativeUnitTypeBusiness(IAdministrativeUnitTypeRepository administrativeUnitTypeRepository)
		{
			this.administrativeUnitTypeRepository = administrativeUnitTypeRepository;
		}

		#region Read

		public IEnumerable<AdministrativeUnitType> Get()
		{
			return administrativeUnitTypeRepository.Get();
		}

		public IEnumerable<AdministrativeUnitType> GetAdministrativeUnitsTypes()
		{
			return administrativeUnitTypeRepository.GetAdministrativeUnitsTypes();
		}

        #endregion

        #region Write

        public IEnumerable<AdministrativeUnitType> Save(IEnumerable<AdministrativeUnitType> unitTypes)
        {
            return administrativeUnitTypeRepository.Save(unitTypes);
        }

        #endregion
    }
}
