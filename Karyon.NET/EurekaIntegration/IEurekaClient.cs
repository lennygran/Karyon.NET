using System;
using System.Threading.Tasks;

namespace Karyon.EurekaIntegration
{
    internal interface IEurekaClient
    {
        string ApplicationName { get; set; }
        int ApplicationPort { get; set; }
        int ApplicationSecurePort { get; set; }
        string EurekaPath { get; set; }
        Task<bool> PutInstanceToService(DataCenterMetadata dcData);
        Task<bool> Register(DataCenterMetadata dcData);
        Task<bool> SendHeartbeat(DataCenterMetadata dcData);
        Task<bool> TakeInstanceOutOfService(DataCenterMetadata dcData);
        Task<bool> Unregister(DataCenterMetadata dcData);
    }
}
