using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Karyon.EurekaIntegration.Tests
{
    public class EurekaClientFake : IEurekaClient
    {
        public bool? _shouldErrorOut;

        public string EurekaServiceUrl { get; set; }
        public string ApplicationName { get; set; }
        public int ApplicationPort { get; set; }
        public int ApplicationSecurePort { get; set; }

        private Task<bool> DoWork(DataCenterMetadata dcData)
        {
            if (!_shouldErrorOut.HasValue)
                throw new System.Net.NetworkInformation.NetworkInformationException();

            var taskSource = new TaskCompletionSource<bool>();
            taskSource.SetResult(_shouldErrorOut.Value);
            return taskSource.Task;
        }

        public Task<bool> PutInstanceToService(DataCenterMetadata dcData)
        {
            return this.DoWork(dcData);
        }

        public Task<bool> Register(DataCenterMetadata dcData)
        {
            return this.DoWork(dcData);
        }

        public Task<bool> SendHeartbeat(DataCenterMetadata dcData)
        {
            return this.DoWork(dcData);
        }

        public Task<bool> TakeInstanceOutOfService(DataCenterMetadata dcData)
        {
            return this.DoWork(dcData);
        }

        public Task<bool> Unregister(DataCenterMetadata dcData)
        {
            return this.DoWork(dcData);
        }
    }
}
