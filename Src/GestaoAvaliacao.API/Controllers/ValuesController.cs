using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Description;

namespace GestaoAvaliacao.API.Controllers
{
	[ApiExplorerSettings(IgnoreApi = true)]
	public class ValuesController : ApiController
	{
		// GET api/values
		public IEnumerable<string> Get()
		{
			return new string[] { "value1", "value2" };
		}

		// GET api/values/5
		public string Get(int id)
		{
			return "value";
		}
	}
}
