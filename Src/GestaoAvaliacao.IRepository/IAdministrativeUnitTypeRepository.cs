using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Util;
using System;
using System.Collections.Generic;

namespace GestaoAvaliacao.IRepository
{
	public interface IAdministrativeUnitTypeRepository
    {
        IEnumerable<AdministrativeUnitType> Get();
        IEnumerable<AdministrativeUnitType> GetAdministrativeUnitsTypes();
        IEnumerable<AdministrativeUnitType> Save(IEnumerable<AdministrativeUnitType> unitTypes);

    }
}
