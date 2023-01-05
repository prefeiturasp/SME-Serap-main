using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Util;
using System.Collections.Generic;

namespace GestaoAvaliacao.IRepository
{
    public interface IResultadoPspRepository
    {
        IEnumerable<ArquivoResultadoPsp> ObterImportacoes(ref Pager pager, string codigoOuNomeArquivo);
    }
}
