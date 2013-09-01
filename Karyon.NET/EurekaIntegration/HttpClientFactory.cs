using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;

namespace Karyon.EurekaIntegration
{
    internal interface IHttpClientFactory
    {
        HttpClient CreateInstance();
    }

    internal class HttpClientFactory : IHttpClientFactory
    {
        public HttpClientFactory()
        {
        }

        public HttpClient CreateInstance()
        {
            return new HttpClient();
        }
    }
}
