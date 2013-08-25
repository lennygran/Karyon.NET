using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace BatchServiceHost
{
    public class HelloController : ApiController
    {
        // GET api/<controller>
        public string Get()
        {
            return "value1";
        }

        // GET api/<controller>/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        public HttpResponseMessage Post([FromBody]string value)
        {
            HttpResponseMessage msg = new HttpResponseMessage()
            {
                Content = new StringContent(value,
                            System.Text.Encoding.UTF8,
                            "text/html"
                        )
            };
            return msg;
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}