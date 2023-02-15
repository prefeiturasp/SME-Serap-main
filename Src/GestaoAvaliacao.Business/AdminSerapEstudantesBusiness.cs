using GestaoAvaliacao.Entities.DTO.SerapEstudantes;
using GestaoAvaliacao.IBusiness;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace GestaoAvaliacao.Business
{
    public class SerapEstudantesBusiness : ISerapEstudantesBusiness
    {
        public static readonly string ENDPOINT_ADMIN_AUTENTICACAO = "admin/autenticacao";

        private readonly string baseUrl;
        private readonly string chaveSerapProvaApi;

        public SerapEstudantesBusiness()
        {
            this.baseUrl = BuscarConfiguracaoObrigatoria("URL_API_SERAP_ESTUDANTES");
            this.chaveSerapProvaApi = BuscarConfiguracaoObrigatoria("ChaveSerapProvaApi");
        }

        private HttpClient ObterClientConfigurado()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(baseUrl);
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
            using (var client = ObterClientConfigurado())
            {
                adminAutenticacaoDTO.ChaveApi = this.chaveSerapProvaApi;
                HttpResponseMessage response = client.PostAsJsonAsync(ENDPOINT_ADMIN_AUTENTICACAO, adminAutenticacaoDTO).Result;
                response.EnsureSuccessStatusCode();

                AdminAutenticacaoRespostaDTO resposta = response.Content.ReadFromJsonAsync<AdminAutenticacaoRespostaDTO>().Result;

                return resposta;
            }
        }
    }
}
