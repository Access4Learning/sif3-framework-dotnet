using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Sif.Framework.Demo.Provider.Controllers
{
    public class StudentPersonalsController : ApiController
    {
        // GET api/studentpersonals
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/studentpersonals/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/studentpersonals
        public void Post([FromBody]string value)
        {
        }

        // PUT api/studentpersonals/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/studentpersonals/5
        public void Delete(int id)
        {
        }
    }
}
