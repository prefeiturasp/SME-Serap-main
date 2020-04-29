using GestaoAvaliacao.FGVIntegration.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace GestaoAvaliacao.FGVIntegration.FGVEnsinoMedio
{
    public class FGVAPIClient : IFGVAPIClient
    {

        private static readonly string URL_API_FGV_ENSINO_MEDIO = "https://piloto2.ensinomediodigital.fgv.br/api/";
        private static readonly string CLIENTID_API_FGV_ENSINO_MEDIO = "";
        private static string AUTHTOKEN_API_FGV_ENSINO_MEDIO = "";
        private static readonly string[] SECRETKEYS_API_FGV_ENSINO_MEDIO = { "", "" };

        public static readonly string ENDPOINT_AUTH_AUTHORIZE = "auth/authorize";
        public static readonly string ENDPOINT_ESCOLA_REGISTRAR = "escola/registrar";
        public static readonly string ENDPOINT_COORDENADOR_REGISTRAR = "coordenador/registrar";
        public static readonly string ENDPOINT_TURMA_REGISTRAR = "turma/registrar";
        public static readonly string ENDPOINT_PROFESSOR_REGISTRAR = "professor/registrar";
        public static readonly string ENDPOINT_PROFESSOR_INSERIRNATURMA = "professor/inserirnaturma";
        public static readonly string ENDPOINT_ALUNO_REGISTRAR = "aluno/registrar";

        public async Task<ResultadoFGV> SendPost(string pEndPoint, BaseFGVObject pParam)
        {
            var jsonObject = await SendPostGenericResult(pEndPoint, pParam);
            return jsonObject.ToObject<ResultadoFGV>();
        }

        public async Task<JObject> Authenticate()
        {
            var jsonObject = await SendGetGenericResult(ENDPOINT_AUTH_AUTHORIZE);
            int secretKeyIndex = jsonObject["secret_key_index"].Value<int>();
            string expiresDatetime = jsonObject["expires_datetime"].ToString();//ToString() is faster than Value<string>()

            AUTHTOKEN_API_FGV_ENSINO_MEDIO = CreateMD5(CLIENTID_API_FGV_ENSINO_MEDIO + SECRETKEYS_API_FGV_ENSINO_MEDIO[secretKeyIndex] + expiresDatetime);

            return jsonObject;
        }

        private async Task<JObject> SendGetGenericResult(string pEndPoint)
        {
            HttpClient httpClient = CreateDefaultHttpClient();
            string response = await httpClient.GetStringAsync(pEndPoint);
            return JObject.Parse(response);
        }

        private async Task<JObject> SendPostGenericResult(string pEndPoint, BaseFGVObject pParam)
        {
            HttpClient httpClient = CreateDefaultHttpClient();
            HttpResponseMessage response = await httpClient.PostAsJsonAsync<BaseFGVObject>(pEndPoint, pParam);

            string jsonString = await response.Content.ReadAsStringAsync();
            return JObject.Parse(jsonString);
        }

        private static HttpClient CreateDefaultHttpClient()
        {
            HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(URL_API_FGV_ENSINO_MEDIO);

            httpClient.DefaultRequestHeaders.Add("x-ClientID", CLIENTID_API_FGV_ENSINO_MEDIO);
            httpClient.DefaultRequestHeaders.Add("x-AuthToken", AUTHTOKEN_API_FGV_ENSINO_MEDIO);
            return httpClient;
        }

        private static string CreateMD5(string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            // Use input string to calculate MD5 hash
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
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