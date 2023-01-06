using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Util;
using System.Collections.Generic;
using EntityFile = GestaoAvaliacao.Entities.File;

namespace GestaoAvaliacao.IBusiness
{
    public interface IResultadoPspBusiness
    {
        IEnumerable<ArquivoResultadoPsp> ObterImportacoes(ref Pager pager, string codigoOuNomeArquivo);
        IEnumerable<TipoResultadoPsp> ObterTiposResultadoPspAtivos();
        TipoResultadoPsp ObterTipoResultadoPorCodigo(int codigo);
        bool ImportarArquivoResultado(ArquivoResultadoPsp arquivoResultado, EntityFile entity);
    }
}
