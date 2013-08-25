using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Karyon.EurekaIntegration
{
    public class LocalAppConfig
    {
        public string EurekaPath
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["Karyon.NET.NetflixOss.eurekaPath"].ToString();
            }
        }
        public string ApplicationName
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["Karyon.NET.NetflixOss.applicationName"].ToString();
            }
        }
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
        public int ApplicationSecurePort
        {
            get
            {
                int listenToPort = 0;
                int.TryParse(System.Configuration.ConfigurationManager.AppSettings["Karyon.NET.NetflixOss.listenToSecurePort"].ToString(), out listenToPort);
                return listenToPort;
            }
        }
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
        public DataCenterType DataCenter
        {
            get
            {
                if (System.Configuration.ConfigurationManager.AppSettings["Karyon.NET.NetflixOss.datacenter"].ToString().Equals("Amazon", StringComparison.CurrentCultureIgnoreCase))
                    return DataCenterType.Amazon;
                return DataCenterType.MyOwn;
            }
        }
        public string NonAmazonLocalIPv4
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["Karyon.NET.NetflixOss.nonAmazon-localIPv4"].ToString();
            }
        }
        public string NonAmazonPublicIPv4
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["Karyon.NET.NetflixOss.nonAmazon-publicIPv4"].ToString();
            }
        }
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
