using GestaoAvaliacao.Dtos;
using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Entities.DTO;
using GestaoAvaliacao.Entities.Enumerator;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.IRepository;
using GestaoAvaliacao.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Web.Configuration;

namespace GestaoAvaliacao.Business
{
    public class ExportAnalysisBusiness : IExportAnalysisBusiness
    {
        #region Dependences

        readonly IExportAnalysisRepository exportAnalysisRepository;
        private readonly ITestBusiness testBusiness;

        #endregion        

        public ExportAnalysisBusiness(IExportAnalysisRepository exportAnalysisRepository, ITestBusiness testBusiness)
        {
            this.exportAnalysisRepository = exportAnalysisRepository;
            this.testBusiness = testBusiness;
        }

        public ExportAnalysis Save(ExportAnalysis entity)
        {
            entity.Validate = this.Validate(entity);

            if (entity.Validate.IsValid)
            {
                var cadastred = exportAnalysisRepository.GetByTestId(entity.Test_Id);

                if (cadastred != null)
                {
                    cadastred.StateExecution = entity.StateExecution;
                    entity = exportAnalysisRepository.Update(cadastred);
                }
                else
                {
                    entity = exportAnalysisRepository.Save(entity);
                }
            }

            return entity;
        }

        public ExportAnalysis Update(ExportAnalysis entity)
        {
            entity.Validate = this.Validate(entity);

            if (entity.Validate.IsValid)
            {
                entity = exportAnalysisRepository.Update(entity);
            }

            return entity;
        }

        public IEnumerable<ExportAnalysisDTO> Search(ref Pager pager, ExportAnalysisFilter filter)
        {
            var provas = new List<ExportAnalysisDTO>();

            switch (filter.Sistema + 1)
            {
                case (short)EnumSystem.SerapOnLine:
                {
                    var provasSerap = exportAnalysisRepository.Search(ref pager, filter);
                    if (provasSerap != null && provasSerap.Any())
                        provas.AddRange(provasSerap);
                    break;
                }
                case (short)EnumSystem.SerapEstudantes:
                {
                    var provasSerapEstudantes = ObterProvasExportacaoSerapEstudantes(ref pager, filter);
                    if (provasSerapEstudantes != null && provasSerapEstudantes.Any())
                        provas.AddRange(provasSerapEstudantes);
                    break;
                }
            }

            return provas;
        }

        public ExportAnalysis SolicitExport(long TestId)
        {
            var entity = new ExportAnalysis() { StateExecution = EnumServiceState.Pending, Test_Id = TestId };
            try
            {
                var provaExportacaoSerapEstudantes = ObterProvaExportacaoSerapEstudantesPorId(TestId);

                // Valida se a prova é do tipo SPA e para estudantes sem deficiência
                if (VerificarSeProvaSpa(provaExportacaoSerapEstudantes) 
                    && provaExportacaoSerapEstudantes.TipoParaEstudanteComDeficiencia == false)
                {
                    // Retona com Erro
                    return RetornaErroValidacaoTipoProva(entity);
                }

                var test = testBusiness.GetObjectWithTestType(TestId);

                if (test != null && test.ShowOnSerapEstudantes )
                {
                    SolicitarExportacaoProvaSerapEstudantes(TestId);
                    entity.Validate.Type = "OK";
                    entity.Validate.Message = null;
                }
                else
                {
                    entity = Save(entity);
                }
                return entity;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void SolicitarExportacaoProvaSerapEstudantes(long TestId)
        {
            try
            {
                var client = ObterClientConfigurado();
                HttpResponseMessage response = client.GetAsync($"exportacoes-resultados/{TestId}/exportar").GetAwaiter().GetResult();
                if (!response.IsSuccessStatusCode)
                {
                    throw new ArgumentException($"Erro ao solicitar exportação da prova: {TestId}, no serap estudantes.");
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public byte[] SolicitarDownloadArquivoSerapEstudantes(long ProcessoId)
        {
            try
            {
                var client = ObterClientConfigurado();
                var resposta = client.GetAsync($"exportacoes-resultados/{ProcessoId}/download").GetAwaiter().GetResult();
                if (resposta.IsSuccessStatusCode)
                {
                    return resposta.Content.ReadAsByteArrayAsync().GetAwaiter().GetResult();
                }
                throw new ArgumentException("Não foi possível baixar o arquivo na api serap estudantes.");
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<ExportAnalysisDTO> ObterProvasExportacaoSerapEstudantes(ref Pager pager, ExportAnalysisFilter filter)
        {
            try
            {
                var client = ObterClientConfigurado();
                var requestMessage = new HttpRequestMessage(new HttpMethod("POST"), $"{client.BaseAddress.AbsoluteUri}exportacoes-resultados/exportacoes-status");
                var filtro = new FiltroExportacaoResultadoDto { ProvaSerapId = filter.Code, DescricaoProva = filter.DescricaoProva, DataInicio = filter.StartDate, DataFim = filter.EndDate, QuantidadeRegistros = pager.PageSize, NumeroPagina = pager.CurrentPage };

                requestMessage.Content = new StringContent(JsonSerializer.Serialize(filtro), Encoding.UTF8, "application/json");
                requestMessage.Headers.Accept.Clear();
                requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("text/plain"));

                var response = client.SendAsync(requestMessage).GetAwaiter().GetResult();

                PaginacaoResultadoDto<ExportAnalysisDTO> provasSerapEstudantes;
                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    var optionsJson = new JsonSerializerOptions { PropertyNameCaseInsensitive = true, WriteIndented = true };
                    provasSerapEstudantes = JsonSerializer.Deserialize<PaginacaoResultadoDto<ExportAnalysisDTO>>(result, optionsJson);

                    pager.SetTotalPages(provasSerapEstudantes.TotalPaginas);
                    pager.SetTotalItens(provasSerapEstudantes.TotalRegistros);
                }
                else
                {
                    var statusCode = response.StatusCode.ToString();
                    var responseMessage = response.EnsureSuccessStatusCode();
                    throw new Exception($"erro ao obter provas para exportar - StatusCode:{statusCode}, Content:{responseMessage.Content}");
                }

                return provasSerapEstudantes.Items.ToList();
            }
            catch (Exception e)
            {
                throw new Exception($"erro ao obter provas para exportar - {e.Message}");
            }
        }

        private HttpClient ObterClientConfigurado()
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

        public IEnumerable<ExportAnalysis> GetByExecutionState(EnumServiceState state)
        {
            return exportAnalysisRepository.GetByExecutionState(state);
        }


        #region Private methods

        private ExportAnalysisDTO ObterProvaExportacaoSerapEstudantesPorId(long testId)
        {
            try
            {
                var pager = new Pager();
                var filter = new ExportAnalysisFilter { Code = testId };
                var provas = ObterProvasExportacaoSerapEstudantes(ref pager, filter);
                return provas?.FirstOrDefault();
            }
            catch
            {
                return null;
            }
        }

        private bool VerificarSeProvaSpa(ExportAnalysisDTO provaExportacao)
        {
            if (provaExportacao == null || string.IsNullOrEmpty(provaExportacao.TipoDescricao))
                return false;

            const string tipoProvaSpa = "Prova Saberes e Aprendizagens";
            string tipoDescricaoNormalizada = NormalizarTexto(provaExportacao.TipoDescricao);
            bool isProvaSpa = tipoDescricaoNormalizada.Contains(NormalizarTexto(tipoProvaSpa));

            return isProvaSpa;
        }

        // Remove espaços no início e fim, substitui espaços múltiplos por um único espaço e converte para lowercase
        private string NormalizarTexto(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return string.Empty;

            return System.Text.RegularExpressions.Regex.Replace(texto.ToLowerInvariant(), @"\s+", "");
        }

        private ExportAnalysis RetornaErroValidacaoTipoProva(
            ExportAnalysis entity)
        {
            entity.Validate.IsValid = false;
            entity.Validate.Type = ValidateType.error.ToString();

            entity.Validate.Message = $"O relatório solicitado deve ser extraído na tela de 'Boletim de Provas'.";

            return entity;
        }

        private Validate Validate(ExportAnalysis entity)
        {
            Validate valid = new Util.Validate();

            if (entity.Test_Id == 0)
                valid.Message = "Prova é obrigatório.";

            if (!string.IsNullOrEmpty(valid.Message))
            {
                string br = "<br/>";
                valid.Message = valid.Message.TrimStart(br.ToCharArray());

                valid.IsValid = false;

                if (valid.Code <= 0)
                    valid.Code = 400;

                valid.Type = ValidateType.alert.ToString();
            }
            else
                valid.IsValid = true;

            return valid;
        }
        #endregion
    }
}
