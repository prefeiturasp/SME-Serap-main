using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ProvaSP.Web.Controllers
{
    public class ValuesController : ApiController
    {
        // GET api/values
        public IEnumerable<string> Get()
        {
            return new string[] { "System.Diagnostics.Debugger.IsAttached:", System.Diagnostics.Debugger.IsAttached.ToString() };
        }

        // GET api/values/5
        public string Get(int id)
        {
            return Data.DataDRE.RecuperarCodigoDREComBaseNoPerfilDaPessoa("2018", Guid.NewGuid().ToString());
        }

        // POST api/values
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
