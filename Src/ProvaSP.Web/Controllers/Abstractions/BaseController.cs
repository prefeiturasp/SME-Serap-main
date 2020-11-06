using Newtonsoft.Json;
using ProvaSP.Web.Services.Abstractions;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace ProvaSP.Web.Controllers.Abstractions
{
    public abstract class BaseController : ApiController
    {
        protected HttpResponseMessage GetResponse<TResult>(TResult result)
            where TResult : BaseDto 
            => result.Valid ? GetValidResponse(result) : GetErrorResponse(result);

        private HttpResponseMessage GetValidResponse<TResult>(TResult result)
            where TResult : BaseDto
        {
            var resultJson = JsonConvert.SerializeObject(result);
            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(resultJson, Encoding.UTF8, "application/json");
            return response;
        }

        private HttpResponseMessage GetErrorResponse<TResult>(TResult result)
            where TResult : BaseDto
        {
            var message = string.Join(" ", result.ErrorMessages);
            return new HttpResponseMessage(HttpStatusCode.ExpectationFailed)
            {
                ReasonPhrase = message.Replace(Environment.NewLine, " ")
            };
        }

        protected HttpResponseMessage GetErrorResponse(Exception ex)
        {
            var message = ex.InnerException?.Message ?? ex.Message;

            return new HttpResponseMessage(HttpStatusCode.ExpectationFailed)
            {
                ReasonPhrase = message.Replace(Environment.NewLine, " ")
            };
        }
    }
}