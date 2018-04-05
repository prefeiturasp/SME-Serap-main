using GestaoEscolar.Entities;
using System.Collections.Generic;

namespace GestaoEscolar.IRepository
{
    public interface IACA_TipoModalidadeEnsinoRepository
    {
        IEnumerable<ACA_TipoModalidadeEnsino> Load();
        ACA_TipoModalidadeEnsino Get(long id);
    }
}
