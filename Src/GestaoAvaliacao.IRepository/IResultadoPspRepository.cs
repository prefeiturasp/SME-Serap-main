using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Util;
using System.Collections.Generic;

namespace GestaoAvaliacao.IRepository
{
    public interface IResultadoPspRepository
    {        
        IEnumerable<ArquivoResultadoPsp> ObterImportacoes(ref Pager pager);
        IEnumerable<ArquivoResultadoPsp> ObterImportacoes(string codigoOuNomeArquivo);
        ArquivoResultadoPsp InserirNovo(ArquivoResultadoPsp arquivoResultado);
        bool ExcluirPorId(long id);
    }
}
