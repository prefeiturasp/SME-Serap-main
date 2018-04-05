using GestaoEscolar.Entities;
using System.Collections.Generic;

namespace GestaoEscolar.IBusiness
{
    public interface IACA_TipoNivelEnsinoBusiness
    {
        IEnumerable<ACA_TipoNivelEnsino> Load();
        ACA_TipoNivelEnsino Get(long id);
        object GetCustom(long id);
    }
}
