using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Util;
using System.Collections.Generic;
using System.Web;

namespace GestaoAvaliacao.IBusiness
{
    public interface IResultadoPspBusiness
    {
        IEnumerable<ArquivoResultadoPsp> ObterImportacoes(ref Pager pager, string codigoOuNomeArquivo);
        IEnumerable<TipoResultadoPsp> ObterTiposResultadoPspAtivos();
        TipoResultadoPsp ObterTipoResultadoPorCodigo(int codigo);
        bool ImportarArquivoResultado(ArquivoResultadoPsp arquivoResultado, HttpPostedFileBase file);
    }
}
