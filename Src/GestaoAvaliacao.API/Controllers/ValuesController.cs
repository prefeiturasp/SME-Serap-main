using System.Collections.Generic;
using System.Web.Http;

namespace GestaoAvaliacao.API.Controllers
{
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
