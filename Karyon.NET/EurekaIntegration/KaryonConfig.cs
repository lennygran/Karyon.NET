using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Collections.Specialized;
using System.Xml;

namespace Karyon.EurekaIntegration
{
    [Serializable()]
    public class KaryonConfigHandler : IConfigurationSectionHandler
    {
        public KaryonConfigHandler()
        {
        }

        public object Create(object parent, object configContext, System.Xml.XmlNode section)
        {
            XmlElement root = (XmlElement)section;
            return new KaryonConfig(root);
        }
    }

    [Serializable()]
    public class KaryonConfig
    {
        /// <summary>
        /// The list of full URLs to the Eureka service.
        /// </summary>
        public List<string> EurekaServiceUrls { get; private set; }
        /// <summary>
        /// The application name. The name should not contain special symbols and/or white spaces.
        /// </summary>
        public string ApplicationName { get; private set; }
        /// <summary>
        /// The non-secure port that application should listen with its services.
        /// </summary>
        public int ApplicationPort { get; private set; }
        /// <summary>
        /// The secure port that application should listen with its services. 0 means the application does not listen to secure port.
        /// </summary>
        public int ApplicationSecurePort { get; private set; }
        /// <summary>
        /// A boolean flag indicated that application should listen to public IP address. If true, the public IP is used, if false, the private IP is used.
        /// </summary>
        public bool ListenToPublic { get; private set; }
        /// <summary>
        /// Gets the Data Center name.
        /// </summary>
        public DataCenterType DataCenter { get; private set; }

        /// <summary>
        /// In case when non-Amazon data center is used, the Local IP address that should be used by the application.
        /// </summary>
        public string NonAmazonLocalIPv4 { get; private set; }
        /// <summary>
        /// In case when non-Amazon data center is used, the Public IP address that should be used by the application.
        /// </summary>
        public string NonAmazonPublicIPv4 { get; private set; }
        /// <summary>
        /// In case when non-Amazon data center is used, the InstanceId that should be used by the application.
        /// </summary>
        public string NonAmazonInstanceId { get; private set; }

        public KaryonConfig(XmlElement xmlData)
        {
            if (xmlData == null)
                throw new ArgumentNullException("Xml configuration element cannot be null.");

            XmlNode oNode = xmlData.SelectSingleNode("datacenter");
            if (oNode == null)
                throw new ApplicationException("Cannot find an <datacenter> node in karyon.net.config xml.");

            XmlNode oSubNode = oNode.Attributes.GetNamedItem("name");
            if (oSubNode == null)
                throw new ApplicationException("Cannot find an <datacenter/@name> attribute in karyon.net.config xml.");
            if (oSubNode.InnerText.Trim().Equals("Amazon", StringComparison.CurrentCultureIgnoreCase))
                this.DataCenter = DataCenterType.Amazon;
            else
            {
                this.DataCenter = DataCenterType.MyOwn;
                oSubNode = oNode.SelectSingleNode("localIPv4");
                if (oSubNode == null)
                    throw new ApplicationException("Cannot find an <datacenter/localIPv4> node in karyon.net.config xml.");
                this.NonAmazonLocalIPv4 = oSubNode.InnerText.Trim();

                oSubNode = oNode.SelectSingleNode("publicIPv4");
                if (oSubNode == null)
                    throw new ApplicationException("Cannot find an <datacenter/publicIPv4> node in karyon.net.config xml.");
                this.NonAmazonPublicIPv4 = oSubNode.InnerText.Trim();

                oSubNode = oNode.SelectSingleNode("instanceID");
                if (oSubNode == null)
                    throw new ApplicationException("Cannot find an <datacenter/instanceID> node in karyon.net.config xml.");
                this.NonAmazonInstanceId = oSubNode.InnerText.Trim();
            }

            oNode = xmlData.SelectSingleNode("applicationName");
            if (oNode == null)
                throw new ApplicationException("Cannot find an <applicationName> node in karyon.net.config xml.");
            this.ApplicationName = oNode.InnerText.Trim();

            oNode = xmlData.SelectSingleNode("listenTo");
            if (oNode == null)
                throw new ApplicationException("Cannot find an <listenTo> node in karyon.net.config xml.");

            oSubNode = oNode.Attributes.GetNamedItem("port");
            if (oSubNode == null)
                throw new ApplicationException("Cannot find an <listenTo/@port> attribute in karyon.net.config xml.");
            this.ApplicationPort = int.Parse(oSubNode.InnerText.Trim());

            oSubNode = oNode.Attributes.GetNamedItem("securePort");
            if (oSubNode == null)
                throw new ApplicationException("Cannot find an <listenTo/@securePort> attribute in karyon.net.config xml.");
            this.ApplicationSecurePort = int.Parse(oSubNode.InnerText.Trim());

            oSubNode = oNode.Attributes.GetNamedItem("isPublic");
            if (oSubNode == null)
                throw new ApplicationException("Cannot find an <listenTo/@isPublic> attribute in karyon.net.config xml.");
            this.ListenToPublic = bool.Parse(oSubNode.InnerText.Trim());

            oNode = xmlData.SelectSingleNode("eurekaServiceUrl");
            if (oNode == null)
                throw new ApplicationException("Cannot find an <eurekaServiceUrl> node in karyon.net.config xml.");
            this.EurekaServiceUrls = new List<string>();
            XmlNodeList oSubNodes = oNode.SelectNodes("add");
            if (oSubNodes == null)
                throw new ApplicationException("Cannot find an <eurekaServiceUrl/add> node in karyon.net.config xml.");
            foreach (XmlNode node in oSubNodes)
            {
                if (node.InnerText.Trim().Length > 0)
                    this.EurekaServiceUrls.Add(node.InnerText.Trim());
            }
            if (this.EurekaServiceUrls.Count == 0)
                throw new ApplicationException("At least one EurekaServiceUrl must be configured in karyon.net.config xml.");
        }

        /// <summary>
        /// Gets current Karyon.NET configuration.
        /// </summary>
        public static KaryonConfig Current
        {
            get
            {
                return (KaryonConfig)ConfigurationManager.GetSection("karyon.net.config");
            }
        }
    }
}