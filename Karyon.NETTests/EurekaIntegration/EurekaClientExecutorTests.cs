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
    public class EurekaClientExecutorTests
    {
        [TestMethod()]
        public void ExecuteWithRetryTest_Retry2Times()
        {
            IEurekaClient eureka = new EurekaClientFake()
            {
                EurekaServiceUrl = KaryonConfig.Current.EurekaServiceUrls[0],
                ApplicationName = KaryonConfig.Current.ApplicationName,
                ApplicationPort = KaryonConfig.Current.ApplicationPort,
                ApplicationSecurePort = KaryonConfig.Current.ApplicationSecurePort
            };

            bool result = EurekaClientExecutor.Wrap(eureka).ExecuteWithRetry(() => eureka.Register(null), KaryonConfig.Current.EurekaServiceUrls);

            Assert.AreEqual(KaryonConfig.Current.EurekaServiceUrls[1], eureka.EurekaServiceUrl);
            Assert.AreEqual(false, result);
        }

        [TestMethod()]
        public void ExecuteWithRetryTest_Try1TimeSuccess()
        {
            IEurekaClient eureka = new EurekaClientFake()
            {
                EurekaServiceUrl = KaryonConfig.Current.EurekaServiceUrls[0],
                ApplicationName = KaryonConfig.Current.ApplicationName,
                ApplicationPort = KaryonConfig.Current.ApplicationPort,
                ApplicationSecurePort = KaryonConfig.Current.ApplicationSecurePort
            };
            (eureka as EurekaClientFake)._shouldErrorOut = true;

            bool result = EurekaClientExecutor.Wrap(eureka).ExecuteWithRetry(() => eureka.Register(null), KaryonConfig.Current.EurekaServiceUrls);

            Assert.AreEqual(KaryonConfig.Current.EurekaServiceUrls[0], eureka.EurekaServiceUrl);
            Assert.AreEqual(true, result);
        }

        [TestMethod()]
        public void ExecuteWithRetryTest_Try1TimeFail()
        {
            IEurekaClient eureka = new EurekaClientFake()
            {
                EurekaServiceUrl = KaryonConfig.Current.EurekaServiceUrls[0],
                ApplicationName = KaryonConfig.Current.ApplicationName,
                ApplicationPort = KaryonConfig.Current.ApplicationPort,
                ApplicationSecurePort = KaryonConfig.Current.ApplicationSecurePort
            };
            (eureka as EurekaClientFake)._shouldErrorOut = false;

            bool result  = EurekaClientExecutor.Wrap(eureka).ExecuteWithRetry(() => eureka.Register(null), KaryonConfig.Current.EurekaServiceUrls);

            Assert.AreEqual(KaryonConfig.Current.EurekaServiceUrls[0], eureka.EurekaServiceUrl);
            Assert.AreEqual(false, result);
        }

        [TestMethod()]
        public void ExecuteWithRetryTest_Try3TimesFail()
        {
            IEurekaClient eureka = new EurekaClientFake()
            {
                EurekaServiceUrl = KaryonConfig.Current.EurekaServiceUrls[0],
                ApplicationName = KaryonConfig.Current.ApplicationName,
                ApplicationPort = KaryonConfig.Current.ApplicationPort,
                ApplicationSecurePort = KaryonConfig.Current.ApplicationSecurePort
            };
            (eureka as EurekaClientFake)._shouldErrorOut = null;

            bool result  = EurekaClientExecutor.Wrap(eureka).ExecuteWithRetry(() => eureka.Register(null), 3);

            Assert.AreEqual(false, result);
        }
    }
}
