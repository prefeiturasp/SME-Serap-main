using GestaoEscolar.Entities;
using System.Collections.Generic;

namespace GestaoEscolar.IBusiness
{
    public interface IACA_TipoModalidadeEnsinoBusiness
    {
        IEnumerable<ACA_TipoModalidadeEnsino> Load();
        ACA_TipoModalidadeEnsino Get(long id);
        object GetCustom(long id);
    }
}
