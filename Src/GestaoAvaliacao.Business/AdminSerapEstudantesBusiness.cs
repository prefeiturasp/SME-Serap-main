using GestaoAvaliacao.Entities.DTO.SerapEstudantes;
using GestaoAvaliacao.IBusiness;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace GestaoAvaliacao.Business
{
    public class SerapEstudantesBusiness : ISerapEstudantesBusiness
    {
        private const string ENDPOINT_ADMIN_AUTENTICACAO = "admin/autenticacao";
        private const string ENDPOINT_SIMULADOR_AUTENTICACAO = "simulador/autenticacao";

        private readonly string baseUrlApiSerapEstudantes;
        private readonly string chaveSerapProvaApi;
        private readonly string baseUrlSimuladorSerapEstudantes;
        private readonly string chaveSimuladorProvaApi;

        public SerapEstudantesBusiness()
        {
            baseUrlApiSerapEstudantes = BuscarConfiguracaoObrigatoria("URL_API_SERAP_ESTUDANTES");
            chaveSerapProvaApi = BuscarConfiguracaoObrigatoria("ChaveSerapProvaApi");
            baseUrlSimuladorSerapEstudantes = BuscarConfiguracaoObrigatoria("URL_SIMULADOR_SERAP_ESTUDANTES");
            chaveSimuladorProvaApi = BuscarConfiguracaoObrigatoria("ChaveSimuladorProvaApi");
        }

        private static HttpClient ObterClientConfigurado(string baseUrl)
        {
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

            var client = new HttpClient
            {
                BaseAddress = new Uri(baseUrl)
            };

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/plain"));

            return client;
        }

        private static string BuscarConfiguracaoObrigatoria(string pConfigName)
        {
            var configValue = System.Web.Configuration.WebConfigurationManager.AppSettings[pConfigName];

            if (string.IsNullOrWhiteSpace(configValue))
                throw new ApplicationException($"Necessário configurar a chave '{pConfigName}' no Web.config");

            return configValue;
        }

        public AdminAutenticacaoRespostaDTO AdminAutenticacao(AdminAutenticacaoDTO adminAutenticacaoDTO)
        {
            using (var client = ObterClientConfigurado(baseUrlApiSerapEstudantes))
            {
                adminAutenticacaoDTO.ChaveApi = chaveSerapProvaApi;
                var response = client.PostAsJsonAsync(ENDPOINT_ADMIN_AUTENTICACAO, adminAutenticacaoDTO).Result;
                response.EnsureSuccessStatusCode();

                var resposta = response.Content.ReadFromJsonAsync<AdminAutenticacaoRespostaDTO>().Result;

                return resposta;
            }
        }

        public SimuladorAutenticacaoRespostaDTO SimuladorAutenticacao(SimuladorAutenticacaoDTO simuladorAutenticacao)
        {
            using (var client = ObterClientConfigurado(baseUrlSimuladorSerapEstudantes))
            {
                simuladorAutenticacao.ChaveApi = chaveSimuladorProvaApi;
                var response = client.PostAsJsonAsync(ENDPOINT_SIMULADOR_AUTENTICACAO, simuladorAutenticacao).Result;
                response.EnsureSuccessStatusCode();

                var resposta = response.Content.ReadFromJsonAsync<SimuladorAutenticacaoRespostaDTO>().Result;

                return resposta;
            }
        }
    }
}
