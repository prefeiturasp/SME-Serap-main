using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using GestaoAvaliacao.Entities.DTO.SerapEstudantes;
using GestaoAvaliacao.IBusiness;
using System.Net.Http.Json;

namespace GestaoAvaliacao.Business
{
    public class BoletimProvaBusiness :  IBoletimProvaBusiness

    {
        public static readonly string ENDPOINT_AUTENTICACAO = "api/v1/autenticacao";

        private string baseUrl;
        private readonly string chaveSerapProvaApi;

        public BoletimProvaBusiness()
        {
            this.baseUrl = BuscarConfiguracaoObrigatoria("URL_BOLETIM_PROVA");
            this.chaveSerapProvaApi = BuscarConfiguracaoObrigatoria("ChaveSerapProvaApi");
        }

        private HttpClient ObterClientConfigurado(string ChaveApi)
        {
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => { return true; };

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

                HttpResponseMessage response = client.PostAsJsonAsync(ENDPOINT_AUTENTICACAO, adminAutenticacaoDTO).Result;
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