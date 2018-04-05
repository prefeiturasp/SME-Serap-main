using MSTech.Security.Cryptography;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using System.Web.Configuration;

namespace OMRService
{
    public class ConfigurationService : ConfigurationSection
	{
		#region Authentication Information

		private static ConfigurationService _instance;

		public static ConfigurationService Instance
		{
			get
			{
				if (ConfigurationManager.GetSection("OMRService") == null)
					return _instance ?? (_instance = (ConfigurationService)WebConfigurationManager.GetSection("OMRService"));
				else
					return _instance ?? (_instance = (ConfigurationService)ConfigurationManager.GetSection("OMRService"));

			}
		}

		[ConfigurationProperty("name", IsRequired = true)]
		public string Name
		{
			get { return (string)base["name"]; }
			set { base["name"] = value; }
		}

		[ConfigurationProperty("uri", IsRequired = true)]
		public string Uri
		{
			get { return (string)base["uri"]; }
			set { base["uri"] = value; }
		}

		[ConfigurationProperty("user", IsRequired = true)]
		public string Login
		{
			get { return (string)base["user"]; }
			set { base["user"] = value; }
		}

		[ConfigurationProperty("password", IsRequired = true)]
		public string Password
		{
			get {
				SymmetricAlgorithm cripto = new SymmetricAlgorithm(MSTech.Security.Cryptography.SymmetricAlgorithm.Tipo.TripleDES);
				return cripto.Decrypt((string)base["password"]);
			}
			set { base["password"] = value; }
		}

		#endregion

		#region Custom methods

		public static string GetErrorMessage(HttpResponseMessage response)
		{
			var ser = new System.Web.Script.Serialization.JavaScriptSerializer();
			var message = (string)((Dictionary<string, object>)ser.Deserialize<dynamic>(response.Content.ReadAsStringAsync().Result))["Message"];

			return message.Replace("\"", "");
		}

		public static HttpClient HttpClientInicialize(bool paginate = false, string accept = "application/json", string contentType = "application/json")
		{
			var httpClientHandler = new HttpClientHandler
			{
				UseProxy = false
			};

			HttpClient client = new HttpClient(httpClientHandler);

			client.BaseAddress = new Uri(AuthenticationService.GetUri());
			client.DefaultRequestHeaders.Accept.Clear();

			//set time for timeout 
			client.Timeout = TimeSpan.FromMinutes(40);

			client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(accept));

            if ((!string.IsNullOrEmpty(contentType)) && paginate)
            {
                string currentPage = HttpContext.Current.Request.Headers["CurrentPage"] ?? "0";
                string pageSize = HttpContext.Current.Request.Headers["PageSize"] ?? "10";

                client.DefaultRequestHeaders.Add("CurrentPage", currentPage);
                client.DefaultRequestHeaders.Add("PageSize", pageSize);
            }

			return client;
		}

		public static T Post<T>(string route, string body, AuthenticationService auth)
		{
			using (var client = ConfigurationService.HttpClientInicialize())
			{
				if (!string.IsNullOrEmpty(auth.AccessToken))
					client.DefaultRequestHeaders.Add("x-access-token", auth.AccessToken);

				var result = client.PostAsync(route, new StringContent(body, Encoding.UTF8, "application/json")).Result;
				string content = result.Content.ReadAsStringAsync().Result;
				if (!result.IsSuccessStatusCode)
					throw new Exception(ConfigurationService.GetErrorMessage(result));
				else
					return (T)JsonConvert.DeserializeObject(content, typeof(T));
			}
		}

		public static HttpResponseMessage Post(string route, string body, AuthenticationService auth)
		{
			using (var client = ConfigurationService.HttpClientInicialize())
			{
				if (!string.IsNullOrEmpty(auth.AccessToken))
				{
					client.DefaultRequestHeaders.Add("x-access-token", auth.AccessToken);
				}

				return client.PostAsync(route, new StringContent(body, Encoding.UTF8, "application/json")).Result;
			}
		}


		#endregion
	}
}
