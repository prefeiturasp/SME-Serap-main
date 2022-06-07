using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Threading.Tasks;
using System.Web.Configuration;
using Microsoft.AspNetCore.Mvc;
using System.Web.Http.Results;
using System.Net.Http;
using System.Net;
using System.Web.Http;

namespace GestaoAvaliacao.API.Middlewares
{
     [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ChaveAutenticacaoAPI : Attribute, IAsyncActionFilter 
    {
        private const string ChaveIntegracaoHeader = "x-api-eol-key";
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {


            string chaveApi = WebConfigurationManager.AppSettings["ChaveIntegracaoApi"];

            if (!context.HttpContext.Request.Headers.TryGetValue(ChaveIntegracaoHeader, out var chaveRecebida) ||
                !chaveRecebida.Equals(chaveApi))
            {
                var msg = new HttpResponseMessage(HttpStatusCode.Unauthorized) { ReasonPhrase = "Chave Integração inválida" };
                throw new HttpResponseException(msg);
            
            }

            await next();
        }
    }
}