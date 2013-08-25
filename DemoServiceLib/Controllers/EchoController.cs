using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DemoService.Controllers
{
    public class EchoController : ApiController
    {
        // GET api/<controller>
        public HttpResponseMessage Get()
        {
            return new HttpResponseMessage()
                {
                    Content = new StringContent("ack", System.Text.Encoding.UTF8, "text/plain"),
                     StatusCode= HttpStatusCode.OK
                };
        }

        // POST api/<controller>
        public HttpResponseMessage Post([FromBody]string value)
        {
            return new HttpResponseMessage()
            {
                Content = this.Request.Content,
                StatusCode = HttpStatusCode.OK
            };
        }
    }
}