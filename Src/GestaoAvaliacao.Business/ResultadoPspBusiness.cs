using GestaoAvaliacao.Entities;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.IFileServer;
using GestaoAvaliacao.IRepository;
using GestaoAvaliacao.Util;
using System.Collections.Generic;
using EntityFile = GestaoAvaliacao.Entities.File;

namespace GestaoAvaliacao.Business
{

    public class ResultadoPspBusiness : IResultadoPspBusiness
    {

        private readonly IStorage storage;
        private readonly IResultadoPspRepository resultadoPspRepository;
        private readonly ITipoResultadoPspRepository tipoResultadoPspRepository;

        public ResultadoPspBusiness(IStorage storage, IResultadoPspRepository resultadoPspRepository, ITipoResultadoPspRepository tipoResultadoPspRepository)
        {
            this.storage = storage;
            this.resultadoPspRepository = resultadoPspRepository;
            this.tipoResultadoPspRepository = tipoResultadoPspRepository;
        }

        public IEnumerable<ArquivoResultadoPsp> ObterImportacoes(ref Pager pager, string codigoOuNomeArquivo)
        {
            return resultadoPspRepository.ObterImportacoes(ref pager, codigoOuNomeArquivo);
        }

        public IEnumerable<TipoResultadoPsp> ObterTiposResultadoPspAtivos()
        {
            return tipoResultadoPspRepository.ObterTodosAtivos();
        }

        public TipoResultadoPsp ObterTipoResultadoPorCodigo(int codigo)
        {
            return tipoResultadoPspRepository.ObterPorCodigo(codigo);
        }

        public bool ImportarArquivoResultado(ArquivoResultadoPsp arquivoResultado, EntityFile entity)
        {
            //(byte)EnumState.ativo
            return default;
        }

    }
}
