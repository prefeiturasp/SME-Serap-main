using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Util;
using System;
using System.Collections.Generic;

namespace GestaoAvaliacao.IBusiness
{
	public interface IAdministrativeUnitTypeBusiness
    {
        IEnumerable<AdministrativeUnitType> Get();
        IEnumerable<AdministrativeUnitType> GetAdministrativeUnitsTypes();
        IEnumerable<AdministrativeUnitType> Save(IEnumerable<AdministrativeUnitType> unitTypes);
    }
}
