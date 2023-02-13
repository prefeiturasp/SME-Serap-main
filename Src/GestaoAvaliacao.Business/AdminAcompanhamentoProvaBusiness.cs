using GestaoAvaliacao.Entities.DTO.SerapEstudantes;
using GestaoAvaliacao.IBusiness;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace GestaoAvaliacao.Business
{
  public class AdminAcompanhamentoProvaBusiness : IAdminAcompanhamentoProvaBusiness
    {
        public static readonly string ENDPOINT_ADMIN_AUTENTICACAO = "api/v1/autenticacao";

        private  string baseUrl;
        private readonly string chaveSerapProvaApi;

        public AdminAcompanhamentoProvaBusiness()
        {
            this.baseUrl = BuscarConfiguracaoObrigatoria("URL_ADMIN_ACOMPANHAMENTO_PROVA");
            this.chaveSerapProvaApi = BuscarConfiguracaoObrigatoria("ChaveSerapProvaApi");
        }

        private HttpClient ObterClientConfigurado(string ChaveApi)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(baseUrl);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/plain"));
            client.DefaultRequestHeaders.Add("chave-api", ChaveApi);
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
            using (var client = ObterClientConfigurado(this.chaveSerapProvaApi))
            {
                adminAutenticacaoDTO.ChaveApi = this.chaveSerapProvaApi;

                HttpResponseMessage response = client.PostAsJsonAsync(ENDPOINT_ADMIN_AUTENTICACAO, adminAutenticacaoDTO).Result;
                response.EnsureSuccessStatusCode();

                AdminAutenticacaoRespostaDTO resposta = new AdminAutenticacaoRespostaDTO();
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    resposta = response.Content.ReadFromJsonAsync<AdminAutenticacaoRespostaDTO>().Result;
                }
                else if (response.StatusCode != System.Net.HttpStatusCode.Unauthorized)
                {
                    response.EnsureSuccessStatusCode();
                }

                resposta.StatusCode = (int)response.StatusCode;
                return resposta;
            }
        }
    }
}
