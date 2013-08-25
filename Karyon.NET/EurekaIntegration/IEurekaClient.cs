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
        Task PutInstanceToService(DataCenterMetadata dcData);
        Task Register(DataCenterMetadata dcData);
        Task SendHeartbeat(DataCenterMetadata dcData);
        Task TakeInstanceOutOfService(DataCenterMetadata dcData);
        Task Unregister(DataCenterMetadata dcData);
    }
}
