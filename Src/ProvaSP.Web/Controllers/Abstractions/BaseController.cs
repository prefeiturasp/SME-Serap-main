using Newtonsoft.Json;
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
            where TResult : class
        {
            var resultJson = JsonConvert.SerializeObject(result);
            return GetResponse(resultJson);
        }

        protected HttpResponseMessage GetResponse(string result)
        {
            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(result, Encoding.UTF8, "application/json");
            return response;
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