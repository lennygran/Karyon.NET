using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Karyon.EurekaIntegration
{
    public class LocalAppConfig
    {
        /// <summary>
        /// The full URL path to the Eureka service.
        /// </summary>
        public string EurekaPath
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["Karyon.NET.NetflixOss.eurekaPath"].ToString();
            }
        }
        /// <summary>
        /// The application name. The name should not contain special symbols and/or white spaces.
        /// </summary>
        public string ApplicationName
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["Karyon.NET.NetflixOss.applicationName"].ToString();
            }
        }
        /// <summary>
        /// The non-secure port that application should listen with its services.
        /// </summary>
        public int ApplicationPort
        {
            get
            {
                int listenToPort = 80;
                if (!int.TryParse(System.Configuration.ConfigurationManager.AppSettings["Karyon.NET.NetflixOss.listenToPort"].ToString(), out listenToPort))
                    throw new Exception("Cannot parse listenToPort.");
                return listenToPort;
            }
        }
        /// <summary>
        /// The secure port that application should listen with its services. 0 means the application does not listen to secure port.
        /// </summary>
        public int ApplicationSecurePort
        {
            get
            {
                int listenToPort = 0;
                int.TryParse(System.Configuration.ConfigurationManager.AppSettings["Karyon.NET.NetflixOss.listenToSecurePort"].ToString(), out listenToPort);
                return listenToPort;
            }
        }
        /// <summary>
        /// A boolean flag indicated that application should listen to public IP address. If true, the public IP is used, if false, the private IP is used.
        /// </summary>
        public bool ListenToPublic
        {
            get
            {
                bool listenToPublic = true;
                if (!bool.TryParse(System.Configuration.ConfigurationManager.AppSettings["Karyon.NET.NetflixOss.listenToPublic"].ToString(), out listenToPublic))
                    throw new Exception("Cannot parse listenToPublic.");
                return listenToPublic;
            }
        }
        /// <summary>
        /// Gets the Data Center name.
        /// </summary>
        public DataCenterType DataCenter
        {
            get
            {
                if (System.Configuration.ConfigurationManager.AppSettings["Karyon.NET.NetflixOss.datacenter"].ToString().Equals("Amazon", StringComparison.CurrentCultureIgnoreCase))
                    return DataCenterType.Amazon;
                return DataCenterType.MyOwn;
            }
        }

        /// <summary>
        /// In case when non-Amazon data center is used, the Local IP address that should be used by the application.
        /// </summary>
        public string NonAmazonLocalIPv4
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["Karyon.NET.NetflixOss.nonAmazon-localIPv4"].ToString();
            }
        }
        /// <summary>
        /// In case when non-Amazon data center is used, the Public IP address that should be used by the application.
        /// </summary>
        public string NonAmazonPublicIPv4
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["Karyon.NET.NetflixOss.nonAmazon-publicIPv4"].ToString();
            }
        }
        /// <summary>
        /// In case when non-Amazon data center is used, the InstanceId that should be used by the application.
        /// </summary>
        public string NonAmazonInstanceId
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["Karyon.NET.NetflixOss.nonAmazon-instanceID"].ToString();
            }
        }

        public LocalAppConfig()
        {
        }
    }
}
