using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Diagnostics;

namespace Karyon.EurekaIntegration
{
    /// <summary>
    /// Class collects AWS EC2 metadata.
    /// </summary>
    internal class AmazonMetadataAdapter : IMetadataAdapter
    {        
        /// <summary>
        /// More info:
        /// http://docs.aws.amazon.com/AWSEC2/latest/UserGuide/AESDG-chapter-instancedata.html#instancedata-data-retrieval
        /// </summary>
        private static string AmazonMetadataRootUrl = "http://169.254.169.254/latest/meta-data/";

        /// <summary>
        /// Default constructor.
        /// </summary>
        public AmazonMetadataAdapter()
        {
        }

        /// <summary>
        /// Method collects metadata from local AWS EC2 instance.
        /// </summary>
        /// <returns>Returns an instance of AmazonMetadataType class.</returns>
        public async Task<DataCenterMetadata> Collect()
        {
            DataCenterMetadata metadata = new DataCenterMetadata();
            HttpClient client = new HttpClient();
            metadata.AmiLaunchIndex = await GetParameter(client, "ami-launch-index");
            metadata.LocalHostName = await GetParameter(client, "local-hostname");
            metadata.AvailabilityZone = await GetParameter(client, "placement/availability-zone");
            metadata.InstanceId = await GetParameter(client, "instance-id");
            metadata.PublicIPv4 = await GetParameter(client, "public-ipv4");
            metadata.PublicHostName = await GetParameter(client, "public-hostname");
            metadata.AmiManifestPath = await GetParameter(client, "ami-manifest-path");
            metadata.LocalIPv4 = await GetParameter(client, "local-ipv4");
            metadata.HostName = await GetParameter(client, "hostname");
            metadata.AmiId = await GetParameter(client, "ami-id");
            metadata.InstanceType = await GetParameter(client, "instance-type");
            return metadata;
        }

        private async Task<string> GetParameter(HttpClient client, string paramName)
        {
            try
            {
                return await client.GetStringAsync(AmazonMetadataRootUrl + paramName);
            }
            catch (Exception ex)
            {
                Trace.TraceError("Exception for '" + AmazonMetadataRootUrl + paramName + "': " + ex.ToString());
                return "";
            }
        }
    }

}
