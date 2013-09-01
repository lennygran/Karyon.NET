using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Karyon.EurekaIntegration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Karyon.EurekaIntegration.Tests
{
    [TestClass()]
    public class EurekaClientTests
    {
        private HttpClientTestFactory factory;
        private EurekaClient client;
        private DataCenterMetadata dcData;

        [TestInitialize()]
        public void PrepareTest()
        {
            factory = new HttpClientTestFactory();
            client = new EurekaClient(factory)
           {
               ApplicationName = "DemoApp",
               EurekaServiceUrl = "http://ec2-174-129-161-75.compute-1.amazonaws.com/eureka"
           };
             dcData = new DataCenterMetadata() { InstanceId = "i-56755" };
        }

        [TestMethod()]
        public void RegisterTest_OK()
        {
            factory.FakeResponseMessage = new System.Net.Http.HttpResponseMessage() { Content = new StringContent(""), StatusCode = System.Net.HttpStatusCode.NoContent, ReasonPhrase = "No Content" };
            Task<bool> task = Task.Run(() => client.Register(dcData));
            task.Wait();
            Assert.IsTrue(task.Result);
            Assert.AreEqual(HttpMethod.Post, factory.FakeResponseMessage.RequestMessage.Method);
            Assert.IsTrue(factory.FakeResponseMessage.RequestMessage.RequestUri.ToString().StartsWith(client.EurekaServiceUrl + "/v2/apps/" + client.ApplicationName));
        }

        [TestMethod()]
        public void RegisterTest_NotOK()
        {
            factory.FakeResponseMessage = new System.Net.Http.HttpResponseMessage() { Content = new StringContent("Some Error"), StatusCode = System.Net.HttpStatusCode.InternalServerError, ReasonPhrase = "Internal Server Error" };
            Task<bool> task = Task.Run(() => client.Register(dcData));
            task.Wait();
            Assert.IsFalse(task.Result);
            Assert.AreEqual(HttpMethod.Post, factory.FakeResponseMessage.RequestMessage.Method);
            Assert.IsTrue(factory.FakeResponseMessage.RequestMessage.RequestUri.ToString().StartsWith(client.EurekaServiceUrl + "/v2/apps/" + client.ApplicationName));
        }

        [TestMethod()]
        public void UnregisterTest_OK()
        {
            factory.FakeResponseMessage = new System.Net.Http.HttpResponseMessage() { Content = new StringContent(""), StatusCode = System.Net.HttpStatusCode.OK, ReasonPhrase = "OK" };
            Task<bool> task = Task.Run(() => client.Unregister(dcData));
            task.Wait();
            Assert.IsTrue(task.Result);
            Assert.AreEqual(HttpMethod.Delete, factory.FakeResponseMessage.RequestMessage.Method);
            Assert.IsTrue(factory.FakeResponseMessage.RequestMessage.RequestUri.ToString().StartsWith(client.EurekaServiceUrl + "/v2/apps/" + client.ApplicationName + "/" + dcData.InstanceId));
        }

        [TestMethod()]
        public void UnregisterTest_NotOK()
        {
            factory.FakeResponseMessage = new System.Net.Http.HttpResponseMessage() { Content = new StringContent("Some error"), StatusCode = System.Net.HttpStatusCode.NotFound, ReasonPhrase = "Not Found" };
            Task<bool> task = Task.Run(() => client.Unregister(dcData));
            task.Wait();
            Assert.IsFalse(task.Result);
            Assert.AreEqual(HttpMethod.Delete, factory.FakeResponseMessage.RequestMessage.Method);
            Assert.IsTrue(factory.FakeResponseMessage.RequestMessage.RequestUri.ToString().StartsWith(client.EurekaServiceUrl + "/v2/apps/" + client.ApplicationName + "/" + dcData.InstanceId));
        }

        [TestMethod()]
        public void SendHeartbeatTest_OK()
        {
            factory.FakeResponseMessage = new System.Net.Http.HttpResponseMessage() { Content = new StringContent(""), StatusCode = System.Net.HttpStatusCode.OK, ReasonPhrase = "OK" };
            Task<bool> task = Task.Run(() => client.SendHeartbeat(dcData));
            task.Wait();
            Assert.IsTrue(task.Result);
            Assert.AreEqual(HttpMethod.Put, factory.FakeResponseMessage.RequestMessage.Method);
            Assert.IsTrue(factory.FakeResponseMessage.RequestMessage.RequestUri.ToString().StartsWith(client.EurekaServiceUrl + "/v2/apps/" + client.ApplicationName + "/" + dcData.InstanceId));
        }

        [TestMethod()]
        public void SendHeartbeatTest_NotOKNotFound()
        {
            factory.FakeResponseMessage = new System.Net.Http.HttpResponseMessage() { Content = new StringContent("Instance not found"), StatusCode = System.Net.HttpStatusCode.NotFound, ReasonPhrase = "Not Found" };
            Task<bool> task = Task.Run(() => client.SendHeartbeat(dcData));
            task.Wait();
            Assert.IsFalse(task.Result);
            Assert.AreEqual(HttpMethod.Put, factory.FakeResponseMessage.RequestMessage.Method);
            Assert.IsTrue(factory.FakeResponseMessage.RequestMessage.RequestUri.ToString().StartsWith(client.EurekaServiceUrl + "/v2/apps/" + client.ApplicationName + "/" + dcData.InstanceId));
        }

        [TestMethod()]
        public void SendHeartbeatTest_NotOK()
        {
            factory.FakeResponseMessage = new System.Net.Http.HttpResponseMessage() { Content = new StringContent("some error"), StatusCode = System.Net.HttpStatusCode.InternalServerError, ReasonPhrase = "Internal Server Error" };
            Task<bool> task = Task.Run(() => client.SendHeartbeat(dcData));
            task.Wait();
            Assert.IsFalse(task.Result);
            Assert.AreEqual(HttpMethod.Put, factory.FakeResponseMessage.RequestMessage.Method);
            Assert.IsTrue(factory.FakeResponseMessage.RequestMessage.RequestUri.ToString().StartsWith(client.EurekaServiceUrl + "/v2/apps/" + client.ApplicationName + "/" + dcData.InstanceId));
        }

        [TestMethod()]
        public void TakeInstanceOutOfServiceTest_OK()
        {
            factory.FakeResponseMessage = new System.Net.Http.HttpResponseMessage() { Content = new StringContent(""), StatusCode = System.Net.HttpStatusCode.OK, ReasonPhrase = "OK" };
            Task<bool> task = Task.Run(() => client.TakeInstanceOutOfService(dcData));
            task.Wait();
            Assert.IsTrue(task.Result);
            Assert.AreEqual(HttpMethod.Put, factory.FakeResponseMessage.RequestMessage.Method);
            Assert.IsTrue(factory.FakeResponseMessage.RequestMessage.RequestUri.ToString().StartsWith(client.EurekaServiceUrl + "/v2/apps/" + client.ApplicationName + "/" + dcData.InstanceId + "/status?value=OUT_OF_SERVICE"));
        }

        [TestMethod()]
        public void TakeInstanceOutOfServiceTest_NotOK()
        {
            factory.FakeResponseMessage = new System.Net.Http.HttpResponseMessage() { Content = new StringContent(""), StatusCode = System.Net.HttpStatusCode.InternalServerError, ReasonPhrase = "Internal Server Error" };
            Task<bool> task = Task.Run(() => client.TakeInstanceOutOfService(dcData));
            task.Wait();
            Assert.IsFalse(task.Result);
            Assert.AreEqual(HttpMethod.Put, factory.FakeResponseMessage.RequestMessage.Method);
            Assert.IsTrue(factory.FakeResponseMessage.RequestMessage.RequestUri.ToString().StartsWith(client.EurekaServiceUrl + "/v2/apps/" + client.ApplicationName + "/" + dcData.InstanceId + "/status?value=OUT_OF_SERVICE"));
        }

        [TestMethod()]
        public void PutInstanceToServiceTest_OK()
        {
            factory.FakeResponseMessage = new System.Net.Http.HttpResponseMessage() { Content = new StringContent(""), StatusCode = System.Net.HttpStatusCode.OK, ReasonPhrase = "OK" };
            Task<bool> task = Task.Run(() => client.PutInstanceToService(dcData));
            task.Wait();
            Assert.IsTrue(task.Result);
            Assert.AreEqual(HttpMethod.Put, factory.FakeResponseMessage.RequestMessage.Method);
            Assert.IsTrue(factory.FakeResponseMessage.RequestMessage.RequestUri.ToString().StartsWith(client.EurekaServiceUrl + "/v2/apps/" + client.ApplicationName + "/" + dcData.InstanceId + "/status?value=UP"));
        }

        [TestMethod()]
        public void PutInstanceToServiceTest_NotOK()
        {
            factory.FakeResponseMessage = new System.Net.Http.HttpResponseMessage() { Content = new StringContent(""), StatusCode = System.Net.HttpStatusCode.InternalServerError, ReasonPhrase = "Internal Server Error" };
            Task<bool> task = Task.Run(() => client.PutInstanceToService(dcData));
            task.Wait();
            Assert.IsFalse(task.Result);
            Assert.AreEqual(HttpMethod.Put, factory.FakeResponseMessage.RequestMessage.Method);
            Assert.IsTrue(factory.FakeResponseMessage.RequestMessage.RequestUri.ToString().StartsWith(client.EurekaServiceUrl + "/v2/apps/" + client.ApplicationName + "/" + dcData.InstanceId + "/status?value=UP"));
        }
    }
}
