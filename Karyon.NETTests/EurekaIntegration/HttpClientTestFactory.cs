using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Karyon.EurekaIntegration.Tests
{
    public class HttpClientTestFactory : IHttpClientFactory
    {
        public HttpResponseMessage FakeResponseMessage { get; set; }

        public HttpClientTestFactory()
        {
        }

        public HttpClient CreateInstance()
        {
            HttpResponseMessage fakeResponse = this.FakeResponseMessage == null ? new HttpResponseMessage() : this.FakeResponseMessage;
            FakeHttpMessageHandler fakeHandler = new FakeHttpMessageHandler(fakeResponse);
            HttpClient httpClient = new HttpClient(fakeHandler);

            return httpClient;
        }
    }

    public class FakeHttpMessageHandler : HttpMessageHandler
    {
        private HttpResponseMessage response;

        public FakeHttpMessageHandler(HttpResponseMessage response)
        {
            this.response = response;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<HttpResponseMessage>();
            response.RequestMessage = request;
            tcs.SetResult(response);
            return tcs.Task;
        }
    }
}
