using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Karyon.EurekaIntegration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Karyon.EurekaIntegration.Tests
{
    [TestClass()]
    public class KaryonConfigTests
    {
        [TestMethod()]
        public void KaryonConfigTest()
        {
            KaryonConfig config = KaryonConfig.Current;
            Assert.IsNotNull(config);
            Assert.AreEqual(2, config.EurekaServiceUrls.Count);
            Assert.AreEqual(DataCenterType.MyOwn, config.DataCenter);
            Assert.AreEqual("127.0.0.1", config.NonAmazonLocalIPv4);
            Assert.AreEqual("127.0.0.1", config.NonAmazonPublicIPv4);
            Assert.AreEqual("i-12345", config.NonAmazonInstanceId);
            Assert.AreEqual(true, config.ListenToPublic);
            Assert.AreEqual("DemoService", config.ApplicationName);
            Assert.AreEqual(8181, config.ApplicationSecurePort);
            Assert.AreEqual(8080, config.ApplicationPort);
        }
    }
}
