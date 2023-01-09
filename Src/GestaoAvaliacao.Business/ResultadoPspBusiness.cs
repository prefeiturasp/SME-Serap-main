using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Entities.Enumerator;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.IFileServer;
using GestaoAvaliacao.IRepository;
using GestaoAvaliacao.Util;
using System;
using System.Collections.Generic;
using System.IO;
using EntityFile = GestaoAvaliacao.Entities.File;

namespace GestaoAvaliacao.Business
{

    public class ResultadoPspBusiness : IResultadoPspBusiness
    {

        private readonly IResultadoPspRepository resultadoPspRepository;
        private readonly ITipoResultadoPspRepository tipoResultadoPspRepository;
        private readonly IFileRepository fileRepository;
        private readonly IDadosArquivoResultadoPspRepository dadosArquivoResultadoPspRepository;

        public ResultadoPspBusiness(IResultadoPspRepository resultadoPspRepository, 
            ITipoResultadoPspRepository tipoResultadoPspRepository, 
            IFileRepository fileRepository,
            IDadosArquivoResultadoPspRepository dadosArquivoResultadoPspRepository)
        {
            this.resultadoPspRepository = resultadoPspRepository;
            this.tipoResultadoPspRepository = tipoResultadoPspRepository;
            this.fileRepository = fileRepository;
            this.dadosArquivoResultadoPspRepository = dadosArquivoResultadoPspRepository;
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

        public long ImportarArquivoResultado(ArquivoResultadoPsp arquivoResultado, EntityFile entity)
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
                    return result.Id;
                }                
                return 0;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public bool SalvarDadosArquivoResultado(long arquivoResultadoId, string pathArquivo)
        {
            try
            {                
                using (var reader = new StreamReader(pathArquivo))
                {
                    while (!reader.EndOfStream)
                    {
                        var textoLinhaArquivo = reader.ReadLine();
                        if (textoLinhaArquivo != null && !string.IsNullOrEmpty(textoLinhaArquivo))
                        {
                            var dadoArquivo = new DadosArquivoResultadoPsp(arquivoResultadoId, textoLinhaArquivo);
                            dadosArquivoResultadoPspRepository.InserirNovo(dadoArquivo);
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

    }
}
