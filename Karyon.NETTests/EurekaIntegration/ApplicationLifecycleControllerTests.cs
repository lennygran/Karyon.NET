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
    public class ApplicationLifecycleControllerTests
    {
        [TestMethod()]
        public void CreateInstanceTest()
        {
            ApplicationLifecycleController controller = ApplicationLifecycleController.CreateInstance();
            Assert.IsNotNull(controller);
            Assert.IsNotNull(controller.AppConfig);
            Assert.AreEqual(2, controller.AppConfig.EurekaServiceUrls.Count);
            Assert.AreEqual(DataCenterType.MyOwn, controller.AppConfig.DataCenter);
            Assert.AreEqual("127.0.0.1", controller.AppConfig.NonAmazonLocalIPv4);
            Assert.AreEqual("127.0.0.1", controller.AppConfig.NonAmazonPublicIPv4);
            Assert.AreEqual("i-12345", controller.AppConfig.NonAmazonInstanceId);
            Assert.AreEqual(true, controller.AppConfig.ListenToPublic);
            Assert.AreEqual("DemoService", controller.AppConfig.ApplicationName);
            Assert.AreEqual(8181, controller.AppConfig.ApplicationSecurePort);
            Assert.AreEqual(8080, controller.AppConfig.ApplicationPort);

            Assert.IsNotNull(controller.DataCenterMetadata);
            Assert.AreEqual("i-12345", controller.DataCenterMetadata.InstanceId);
        }
    }
}
