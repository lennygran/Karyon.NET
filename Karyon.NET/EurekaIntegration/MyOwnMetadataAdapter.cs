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
    /// Class collects "MyOwn" data center metadata.
    /// </summary>
    internal class MyOwnMetadataAdapter : IMetadataAdapter
    {        
        /// <summary>
        /// Default constructor.
        /// </summary>
        public MyOwnMetadataAdapter()
        {
        }

        /// <summary>
        /// Method collects metadata from local machine.
        /// </summary>
        /// <returns>Returns an instance of AmazonMetadataType class.</returns>
        public Task<DataCenterMetadata> Collect()
        {
            var taskSource = new TaskCompletionSource<DataCenterMetadata>();
            DataCenterMetadata metadata = new DataCenterMetadata();
            metadata.DataCenterName = DataCenterType.MyOwn;
            KaryonConfig appConfig = KaryonConfig.Current;
            metadata.LocalIPv4 = appConfig.NonAmazonLocalIPv4;
            metadata.PublicIPv4 = appConfig.NonAmazonPublicIPv4;
            metadata.InstanceId = appConfig.NonAmazonInstanceId;
            taskSource.SetResult(metadata);
            return taskSource.Task;
        }
    }
}
