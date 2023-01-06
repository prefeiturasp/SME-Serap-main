using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Entities.Enumerator;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.IFileServer;
using GestaoAvaliacao.IRepository;
using GestaoAvaliacao.Util;
using System;
using System.Collections.Generic;
using EntityFile = GestaoAvaliacao.Entities.File;

namespace GestaoAvaliacao.Business
{

    public class ResultadoPspBusiness : IResultadoPspBusiness
    {

        private readonly IResultadoPspRepository resultadoPspRepository;
        private readonly ITipoResultadoPspRepository tipoResultadoPspRepository;
        private readonly IFileRepository fileRepository;

        public ResultadoPspBusiness(IResultadoPspRepository resultadoPspRepository, ITipoResultadoPspRepository tipoResultadoPspRepository, IFileRepository fileRepository)
        {
            this.resultadoPspRepository = resultadoPspRepository;
            this.tipoResultadoPspRepository = tipoResultadoPspRepository;
            this.fileRepository = fileRepository;
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
            try
            {
                arquivoResultado.FileId = entity.Id;
                arquivoResultado.NomeArquivo = entity.Name;
                var result = resultadoPspRepository.InserirNovo(arquivoResultado);
                if (result != null && result.Id > 0)
                {
                    entity.State = (byte)EnumState.inativo;// inativa o arquivo para não aparecer nas demais telas do sistema.
                    fileRepository.Update(entity.Id, entity);
                    return true;
                }                
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

    }
}
