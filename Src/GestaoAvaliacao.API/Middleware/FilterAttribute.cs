using GestaoAvaliacao.Util;
using GestaoAvaliacao.WebProject.Facade;
using Swashbuckle.Swagger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Configuration;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Description;

namespace GestaoAvaliacao.API.Middleware
{
    public class ProducesOperationFilter : IOperationFilter
    {
        public void Apply(Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription)
        {
            if (operation.parameters == null)
                operation.parameters = new List<Parameter>();

            operation.parameters.Add(new Parameter
            {
                name = "keyApi",
                @in = "header",
                type = "string",
                description = "Chave api",
                required = true
            });
        }
    }

    public class AutAttribute : AuthorizeAttribute
    {
        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            try
            {
                var keyHeaderParameter = GetKey(actionContext);
                var keyApi = WebConfigurationManager.AppSettings["ChaveApi"];
                if (keyHeaderParameter == keyApi)
                    return true;

                actionContext.Response = new HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized);
                return false;
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                actionContext.Response = new HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized);
                return false;
            }
        }


        private string GetKey(HttpActionContext actionContext)
        {
            var token = actionContext.ControllerContext.Request.Headers.FirstOrDefault(k => k.Key.Equals(Constants.keyApi)).Value;
            return token.FirstOrDefault();
        }
    }
}
