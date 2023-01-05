using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Util;
using System.Collections.Generic;

namespace GestaoAvaliacao.IBusiness
{
    public interface IResultadoPspBusiness
    {
        IEnumerable<ArquivoResultadoPsp> ObterImportacoes(ref Pager pager, string codigoOuNomeArquivo);
    }
}
