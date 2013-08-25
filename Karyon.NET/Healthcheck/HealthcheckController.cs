using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Karyon.Healthcheck
{
    /// <summary>
    /// Controller provides healthcheck functionality
    /// </summary>
    public class HealthcheckController : ApiController
    {
        // GET api/<controller>
        /// <summary>
        /// Method provides GET response with "ack" text in its content as healthcheck confirmation.
        /// </summary>
        /// <returns>Returns response with "ack" text as a content.</returns>
        public HttpResponseMessage Get()
        {
            return new HttpResponseMessage()
                {
                    Content = new StringContent("ack", System.Text.Encoding.UTF8, "text/plain"),
                     StatusCode= HttpStatusCode.OK
                };
        }
    }
}