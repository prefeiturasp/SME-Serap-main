using GestaoEscolar.Entities;
using System.Collections.Generic;

namespace GestaoEscolar.IRepository
{
    public interface IACA_TipoNivelEnsinoRepository
    {
        IEnumerable<ACA_TipoNivelEnsino> Load();
        ACA_TipoNivelEnsino Get(long id);
    }
}
