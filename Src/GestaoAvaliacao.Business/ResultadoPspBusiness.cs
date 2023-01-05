using GestaoAvaliacao.Entities;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.IFileServer;
using GestaoAvaliacao.IRepository;
using GestaoAvaliacao.Util;
using System.Collections.Generic;

namespace GestaoAvaliacao.Business
{

    public class ResultadoPspBusiness : IResultadoPspBusiness
    {

        private readonly IStorage storage;
        private readonly IResultadoPspRepository resultadoPspRepository;

        public ResultadoPspBusiness(IStorage storage, IResultadoPspRepository resultadoPspRepository)
        {
            this.storage = storage;
            this.resultadoPspRepository = resultadoPspRepository;
        }

        public IEnumerable<ArquivoResultadoPsp> ObterImportacoes(ref Pager pager, string codigoOuNomeArquivo)
        {
            return resultadoPspRepository.ObterImportacoes(ref pager, codigoOuNomeArquivo);
        }
    }
}
