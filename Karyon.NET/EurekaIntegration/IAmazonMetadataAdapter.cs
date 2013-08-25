using System;
using System.Threading.Tasks;

namespace Karyon.EurekaIntegration
{
    internal interface IMetadataAdapter
    {
        Task<DataCenterMetadata> Collect();
    }
}
