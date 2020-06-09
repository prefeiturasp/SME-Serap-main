using GestaoAvaliacao.FGVIntegration.Exceptions;
using GestaoAvaliacao.FGVIntegration.Logging;
using GestaoAvaliacao.FGVIntegration.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GestaoAvaliacao.FGVIntegration.Business
{
    public class FGVAPIConsumer : LoggingBase, IFGVAPIConsumer
    {

        public static readonly string ENDPOINT_AUTH_AUTHORIZE = "auth/authorize";
        public static readonly string ENDPOINT_REDE_REGISTRAR = "rede/registrar";
        public static readonly string ENDPOINT_ESCOLA_REGISTRAR = "escola/registrar";
        public static readonly string ENDPOINT_COORDENADOR_REGISTRAR = "coordenador/registrar";
        public static readonly string ENDPOINT_TURMA_REGISTRAR = "turma/registrar";
        public static readonly string ENDPOINT_PROFESSOR_REGISTRAR = "professor/registrar";
        public static readonly string ENDPOINT_PROFESSOR_INSERIRNATURMA = "professor/inserirnaturma";
        public static readonly string ENDPOINT_ALUNO_REGISTRAR = "aluno/registrar";

        private DateTime ultimaAutenticacao = DateTime.MinValue;
        private string currentExpiresDate = string.Empty; // data de expiração. Padrão de 30 minutos. Formato: dd/MM/yyyy HH:mm:ss
        private string currentAuthToken = string.Empty; // token pronto para autenticação
        private int currentSecretKeyIndex = 0;

        private readonly string baseUrl;
        private readonly string clientId;
        private readonly string[] secretKeys;
        private static readonly SemaphoreSlim semaforoAuth = new SemaphoreSlim(1, 1);

        public FGVAPIConsumer()
        {
            this.baseUrl = ConfigurationHelper.BuscarConfiguracaoObrigatoria("ApiFgvEnsinoMedio_BaseUrl");
            this.clientId = ConfigurationHelper.BuscarConfiguracaoObrigatoria("ApiFgvEnsinoMedio_ClientId");
            this.secretKeys = new string[] {
                ConfigurationHelper.BuscarConfiguracaoObrigatoria("ApiFgvEnsinoMedio_SecretKey1"),
                ConfigurationHelper.BuscarConfiguracaoObrigatoria("ApiFgvEnsinoMedio_SecretKey2"),
                ConfigurationHelper.BuscarConfiguracaoObrigatoria("ApiFgvEnsinoMedio_SecretKey3"),
                ConfigurationHelper.BuscarConfiguracaoObrigatoria("ApiFgvEnsinoMedio_SecretKey4"),
                ConfigurationHelper.BuscarConfiguracaoObrigatoria("ApiFgvEnsinoMedio_SecretKey5"),
                ConfigurationHelper.BuscarConfiguracaoObrigatoria("ApiFgvEnsinoMedio_SecretKey6"),
                ConfigurationHelper.BuscarConfiguracaoObrigatoria("ApiFgvEnsinoMedio_SecretKey7"),
            };

            var validated = ValidateAuthentication();
        }


        public async Task<ResultadoFGV> SendPost(string pEndPoint, BaseFGVObject pParam)
        {
            try
            {
                await ValidateAuthentication();

                try
                {
                    var results = await SendPostJsonResult(pEndPoint, pParam);
                    return results.ToObject<List<ResultadoFGV>>().Single();
                }
                catch (AutenticacaoException)
                {
                    //se deu erro de authenticação, refaz a autenticação e tenta novamente. Se der erro de novo ¯\_(ツ)_/¯ aí deixa seguir seu caminho natural
                    await Authenticate(true);
                    var results = await SendPostJsonResult(pEndPoint, pParam);
                    return results.ToObject<List<ResultadoFGV>>().Single();
                }
            }
            catch (ApplicationException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                Logger.Error($"Erro na requisição '{pEndPoint}'", ex);
                throw new ApplicationException($"Erro na requisição '{pEndPoint}'. Mensagem: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<ResultadoFGV>> SendPostList(string pEndPoint, IEnumerable<BaseFGVObject> pParam)
        {
            try
            {
                await ValidateAuthentication();

                try
                {
                    var results = await SendPostJsonResult(pEndPoint, pParam);
                    return results.ToObject<List<ResultadoFGV>>();
                }
                catch (AutenticacaoException)
                {
                    //se deu erro de authenticação, refaz a autenticação e tenta novamente. Se der erro de novo ¯\_(ツ)_/¯ aí deixa seguir seu caminho natural
                    await Authenticate(true);
                    var results = await SendPostJsonResult(pEndPoint, pParam);
                    return results.ToObject<List<ResultadoFGV>>();
                }
            }
            catch (ApplicationException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                Logger.Error($"Erro na requisição '{pEndPoint}'", ex);
                throw new ApplicationException($"Erro na requisição '{pEndPoint}'. Mensagem: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Valida pelo tempo de 30 minutos de expiração.
        /// Poderia ser enviado uma autenticação toda vez, mas a ideia deste controle é tentar melhorar performance 
        /// e evitar o lock síncrono da autenticação mais o tempo de resposta do request.
        /// </summary>
        /// <returns>Token de autenticação</returns>
        public async Task<bool> ValidateAuthentication()
        {
            //se a autenticação foi a mais de 30 minutos, realiza outra autenticação.
            if (ultimaAutenticacao <= DateTime.Now.AddMinutes(-30))
                await Authenticate(false);

            return true;
        }

        /// <summary>
        /// Realiza a autenticação de forma síncronza.
        /// </summary>
        /// <param name="forced">Enviar True quando se deseja forçar a autenticação independente do tempo de 30 minutos de expiração</param>
        /// <returns>Token de autenticação</returns>
        private async Task<string> Authenticate(bool forced)
        {
            //é usado um semáforo e não lock devido ao async/await
            await semaforoAuth.WaitAsync();
            try
            {
                if (forced || ultimaAutenticacao <= DateTime.Now.AddMinutes(-30))
                {
                    var horaAtual = DateTime.Now;
                    var ultimaDataExpiracao = this.currentExpiresDate;

                    var results = await SendGetJsonResult(ENDPOINT_AUTH_AUTHORIZE, false);

                    this.currentSecretKeyIndex = results["secret_key_index"].Value<int>();
                    this.currentExpiresDate = results["expires_datetime"].ToString();//ToString() is faster than Value<string>()

                    Logger.Info($"Realizada autenticação: Data:{this.currentExpiresDate} Index:{this.currentSecretKeyIndex}");

                    //caso realmente tenha mudado o token, considera como nova autenticação
                    if (ultimaDataExpiracao != this.currentExpiresDate)
                        ultimaAutenticacao = horaAtual;

                    this.currentAuthToken = CreateMD5(
                        this.clientId
                        + this.secretKeys[this.currentSecretKeyIndex - 1]
                        + this.currentExpiresDate).ToLowerInvariant();

                    Logger.Debug($"Token de autenticação: {currentAuthToken}");
                }
            }
            catch (AutenticacaoException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                Logger.Error($"Erro na autenticação.", ex);
                throw new AutenticacaoException($"Erro na autenticação. Mensagem: {ex.Message}", ex);
            }
            finally
            {
                semaforoAuth.Release();
            }
            return currentAuthToken;
        }

        private async Task<JArray> SendPostJsonResult(string pEndPoint, IEnumerable<BaseFGVObject> pParam)
        {
            HttpClient httpClient = CreateDefaultHttpClient();
            HttpResponseMessage httpResponse = await httpClient.PostAsJsonAsync(pEndPoint, pParam.ToArray());
            var response = await httpResponse.Content.ReadAsStringAsync();

            var results = ParseResult(pEndPoint, response);
            return (JArray)results;
        }

        private async Task<JArray> SendPostJsonResult(string pEndPoint, BaseFGVObject pParam)
        {
            if (Logger.IsDebugEnabled)
                Logger.DebugFormat("Enviando Seq={0} para endpoint {1}: {2}", pParam.Seq, pEndPoint, JsonConvert.SerializeObject(pParam));

            HttpClient httpClient = CreateDefaultHttpClient();
            HttpResponseMessage httpResponse = await httpClient.PostAsJsonAsync(pEndPoint, pParam);//new BaseFGVObject[] { pParam });
            var response = await httpResponse.Content.ReadAsStringAsync();

            Logger.DebugFormat("Retorno Seq={0} do endpoint {1}: {2}", pParam.Seq, pEndPoint, response);
            var results = ParseResult(pEndPoint, response);
            return (JArray)results;
        }

        private async Task<JObject> SendGetJsonResult(string pEndPoint, bool requiresAuth)
        {
            if (requiresAuth)
                await ValidateAuthentication();

            HttpClient httpClient = CreateDefaultHttpClient();
            string response = await httpClient.GetStringAsync($"{pEndPoint}?{DateTime.Now:yyyy-MM-dd'T'HH:mm:ss}");

            var results = ParseResult(pEndPoint, response);
            return (JObject)results;
        }

        private JContainer ParseResult(string pEndPoint, string response)
        {
            try
            {
                JObject jObject = JObject.Parse(response);
                var requestResult = jObject.ToObject<ResultadoRequestFGV>();
                if (requestResult.Status == "fail")
                {
                    Logger.Error($"O request '{pEndPoint}' falhou com a mensagem: {requestResult.Message}");
                    throw new ApplicationException($"O request '{pEndPoint}' falhou com a mensagem: {requestResult.Message}");
                }

                return requestResult.Returns;
            }
            catch (JsonReaderException ex)
            {
                Logger.Error($"Retorno do endpoint '{pEndPoint}' não é um JSON válido ou não está no formato esperado.", ex);
                Logger.Error($"Conteudo do Response:\r\n{response}");
                throw new ApplicationException($"Retorno do endpoint '{pEndPoint}' não é um JSON válido ou não está no formato esperado.", ex);
            }
        }

        private HttpClient CreateDefaultHttpClient()
        {
            HttpClient httpClient = new HttpClient()
            {
                BaseAddress = new Uri(baseUrl)
            };

            httpClient.DefaultRequestHeaders.Add("x-ClientID", clientId);
            httpClient.DefaultRequestHeaders.Add("x-AuthToken", currentAuthToken);
            httpClient.DefaultRequestHeaders.Add("ContentyType", "application/json");
            
            return httpClient;
        }

        private static string CreateMD5(string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            // Use input string to calculate MD5 hash
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }

    }

}