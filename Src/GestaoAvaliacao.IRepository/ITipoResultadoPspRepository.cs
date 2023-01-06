using GestaoAvaliacao.Entities;
using System.Collections.Generic;

namespace GestaoAvaliacao.IRepository
{
    public interface ITipoResultadoPspRepository
    {
        IEnumerable<TipoResultadoPsp> ObterTodosAtivos();
        TipoResultadoPsp ObterPorCodigo(int codigoTipoResultado);
    }
}
