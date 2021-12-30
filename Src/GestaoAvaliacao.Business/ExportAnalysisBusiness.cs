using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Entities.DTO;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.IRepository;
using GestaoAvaliacao.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Net;
using System.Web.Configuration;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text;
using GestaoAvaliacao.Dtos;

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

            var provasSerap = exportAnalysisRepository.Search(ref pager, filter);
            provas.AddRange(provasSerap);

            var provasSerapEstudantes = ObterProvasExportacaoSerapEstudantes(filter);
            if (provasSerapEstudantes != null && provasSerapEstudantes.Any())
                provas.AddRange(provasSerapEstudantes);

            return provas;
        }

        public ExportAnalysis SolicitExport(long TestId)
        {
            var entity = new ExportAnalysis() { StateExecution = EnumServiceState.Pending, Test_Id = TestId };
            try
            {
                var test = testBusiness.GetTestBy_Id(TestId);
                if (test != null && test.ShowOnSerapEstudantes)
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

        public List<ExportAnalysisDTO> ObterProvasExportacaoSerapEstudantes(ExportAnalysisFilter filter)
        {
            var provasSerapEstudantes = new List<ExportAnalysisDTO>();
            try
            {
                var client = ObterClientConfigurado();
                HttpRequestMessage requestMessage = new HttpRequestMessage(new HttpMethod("POST"), $"{client.BaseAddress.AbsoluteUri}exportacoes-resultados/exportacoes-status");
                var filtro = new FiltroExportacaoResultadoDto { ProvaSerapId = filter.Code, DataInicio = filter.StartDate, DataFim = filter.EndDate };
                requestMessage.Content = new StringContent(JsonSerializer.Serialize(filtro), Encoding.UTF8, "application/json");
                requestMessage.Headers.Accept.Clear();
                requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("text/plain"));
                HttpResponseMessage response = client.SendAsync(requestMessage).GetAwaiter().GetResult();

                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    var optionsJson = new JsonSerializerOptions { PropertyNameCaseInsensitive = true, WriteIndented = true };
                    provasSerapEstudantes = JsonSerializer.Deserialize<List<ExportAnalysisDTO>>(result, optionsJson);
                }
                return provasSerapEstudantes;
            }
            catch (Exception e)
            {
                return provasSerapEstudantes;
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
