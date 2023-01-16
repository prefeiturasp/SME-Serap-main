using GestaoAvaliacao.Entities;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.IRepository;
using GestaoAvaliacao.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Configuration;

namespace GestaoAvaliacao.Business
{

    public class ResultadoPspBusiness : IResultadoPspBusiness
    {

        private readonly IResultadoPspRepository resultadoPspRepository;
        private readonly ITipoResultadoPspRepository tipoResultadoPspRepository;

        public ResultadoPspBusiness(IResultadoPspRepository resultadoPspRepository,
            ITipoResultadoPspRepository tipoResultadoPspRepository)
        {
            this.resultadoPspRepository = resultadoPspRepository;
            this.tipoResultadoPspRepository = tipoResultadoPspRepository;
        }

        public IEnumerable<ArquivoResultadoPsp> ObterImportacoes(ref Pager pager, string codigoOuNomeArquivo)
        {
            if (!string.IsNullOrEmpty(codigoOuNomeArquivo))
                return resultadoPspRepository.ObterImportacoes(codigoOuNomeArquivo);
            return resultadoPspRepository.ObterImportacoes(ref pager);
        }

        public IEnumerable<TipoResultadoPsp> ObterTiposResultadoPspAtivos()
        {
            return tipoResultadoPspRepository.ObterTodosAtivos();
        }

        public TipoResultadoPsp ObterTipoResultadoPorCodigo(int codigo)
        {
            return tipoResultadoPspRepository.ObterPorCodigo(codigo);
        }

        public bool ImportarArquivoResultado(ArquivoResultadoPsp arquivoResultado, HttpPostedFileBase file)
        {
            long processoId = 0;
            try
            {
                var result = resultadoPspRepository.InserirNovo(arquivoResultado);
                if (result != null && result.Id > 0)
                {
                    processoId = result.Id;
                    var resultApi = UploadArquivoResultadoPsp(file, arquivoResultado.NomeArquivo);

                    if (resultApi)
                        resultApi = PublicarFilaTratarProcessoImportacao(arquivoResultado);
                    else
                        ExcluirProcessoPorId(processoId);

                    return resultApi;
                }
                return false;
            }
            catch (Exception ex)
            {
                ExcluirProcessoPorId(processoId);
                return false;
            }
        }
        public bool ExcluirProcessoPorId(long id)
        {
            try
            {
                if (id > 0)
                    return resultadoPspRepository.ExcluirPorId(id);
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private bool UploadArquivoResultadoPsp(HttpPostedFileBase file, string nomeArquivo)
        {
            try
            {
                var client = ObterHttpClient();
                var bytesArquivo = readFileContents(file);
                var formContent = new MultipartFormDataContent
                {
                    {new StringContent(nomeArquivo),"arquivoResultadoDto.NomeArquivo"},
                    {new StreamContent(new MemoryStream(bytesArquivo)),"arquivo",nomeArquivo}
                };

                HttpResponseMessage response = client.PostAsync($"resultados-psp/upload-arquivo", formContent).GetAwaiter().GetResult();
                if (!response.IsSuccessStatusCode)
                    throw new ArgumentException($"Erro ao importar arquivo no serap estudantes.");

                string stringContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                if (!string.IsNullOrEmpty(stringContent))
                    return Convert.ToBoolean(stringContent);

                return false;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private bool PublicarFilaTratarProcessoImportacao(ArquivoResultadoPsp arquivoResultado)
        {
            try
            {
                var client = ObterHttpClient();
                HttpResponseMessage response = client.GetAsync($"resultados-psp/processo/{arquivoResultado.Id}/tipo-resultado/{arquivoResultado.CodigoTipoResultado}/tratar").GetAwaiter().GetResult();
                if (!response.IsSuccessStatusCode)
                    throw new ArgumentException($"Erro ao iniciar processo importação.");

                string stringContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                if (!string.IsNullOrEmpty(stringContent))
                    return Convert.ToBoolean(stringContent);

                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private byte[] readFileContents(HttpPostedFileBase file)
        {
            Stream fileStream = file.InputStream;
            var mStreamer = new MemoryStream();
            mStreamer.SetLength(fileStream.Length);
            fileStream.Read(mStreamer.GetBuffer(), 0, (int)fileStream.Length);
            mStreamer.Seek(0, SeekOrigin.Begin);
            byte[] fileBytes = mStreamer.GetBuffer();

            #region MemoryStream
            //using (MemoryStream ms = new MemoryStream())
            //{
            //    file.InputStream.CopyTo(ms);
            //    fileBytes = ms.GetBuffer();
            //}
            #endregion

            return fileBytes;
        }

        private HttpClient ObterHttpClient()
        {
            HttpClient client = new HttpClient();
            string uriConfig = WebConfigurationManager.AppSettings["URL_API_SERAP_ESTUDANTES"];
            string nomeChaveApi = "ChaveSerapProvaApi";
            string chaveApi = WebConfigurationManager.AppSettings[nomeChaveApi];
            client.BaseAddress = new Uri(uriConfig);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/plain"));
            client.DefaultRequestHeaders.Add(nomeChaveApi, chaveApi);
            return client;
        }

    }
}
