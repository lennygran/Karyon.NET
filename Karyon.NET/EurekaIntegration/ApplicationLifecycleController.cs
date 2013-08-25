using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Karyon.EurekaIntegration
{
    /// <summary>
    /// Class provides application or service lifecycle and Eureka integration functionality.
    /// </summary>
    public class ApplicationLifecycleController : IDisposable
    {
        private IEurekaClient eureka;
        private HeartbeatObserver heartbeat;

        /// <summary>
        /// Gets the data center metadata for current instance.
        /// </summary>
        public DataCenterMetadata DataCenterMetadata { get; private set; }
        /// <summary>
        /// Gets the application configuration information.
        /// </summary>
        public LocalAppConfig AppConfig { get; private set; }

        /// <summary>
        /// Method creates an instance of the controller. 
        /// Although this controller is not a singleton object, it is recommended to create as least instances per application as possible.
        /// </summary>
        /// <returns>Returns an initialized instance of ApplicationLifecycleController class.</returns>
        public static ApplicationLifecycleController CreateInstance()
        {
            ApplicationLifecycleController lifecycle = new ApplicationLifecycleController();
            lifecycle.AppConfig = new LocalAppConfig();

            IMetadataAdapter adapter = null;
            if (lifecycle.AppConfig.DataCenter == DataCenterType.Amazon)
            {
                Trace.TraceInformation("Collecting AWS EC2 instance metadata...");
                adapter = new AmazonMetadataAdapter();
            }
            else
            {
                Trace.TraceInformation("Using local machine instead of Amazon...");
                adapter = new MyOwnMetadataAdapter();
            }
            lifecycle.DataCenterMetadata = Task.Run(() => adapter.Collect()).Result;

            //create Eureka client
            lifecycle.eureka = new EurekaClient()
            {
                EurekaPath = lifecycle.AppConfig.EurekaPath,
                ApplicationName = lifecycle.AppConfig.ApplicationName,
                ApplicationPort = lifecycle.AppConfig.ApplicationPort,
                ApplicationSecurePort = lifecycle.AppConfig.ApplicationSecurePort
            };

            return lifecycle;
        }

        private ApplicationLifecycleController()
        {
        }

        /// <summary>
        /// Method registers application instance to eureka service.
        /// </summary>
        public void RegisterApplicationInstance()
        {
            this.RegisterApplicationInstance(null);
        }

        /// <summary>
        /// Method registers application instance to eureka service with ability to provide additional healthcheck callback.
        /// </summary>
        /// <param name="healthCheck">External function performing additional validation for the service health. 
        /// Function must return boolean value indicating the service health. 
        /// If FALSE value returned, then no HEALTHY heartbeat is being sent.</param>
        public void RegisterApplicationInstance(Func<bool> healthCheck)
        {
            try
            {
                Trace.TraceInformation("Registering instance with EUREKA service...");
                Task.Run(() => this.eureka.Register(DataCenterMetadata)).Wait();

                Trace.TraceInformation("Initializing instance heartbeat...");
                this.heartbeat = new HeartbeatObserver(this.eureka, this.DataCenterMetadata);
                if (healthCheck != null)
                    this.heartbeat.OnHealthCheck = healthCheck;
                this.heartbeat.InitializeAndStartTimer();
            }
            catch (Exception ex)
            {
                if (this.eureka != null)
                    Task.Run(() => this.eureka.Unregister(DataCenterMetadata)).Wait();
                if (this.heartbeat != null)
                    this.heartbeat.StopTimer();
                throw ex;
            }
        }

        /// <summary>
        /// Method unregisters application instance from eureka service.
        /// </summary>
        public void UnregisterApplicationInstance()
        {
            if (this.eureka != null)
                Task.Run(() => this.eureka.Unregister(DataCenterMetadata)).Wait();
            else
                throw new Exception("Current Eureka client is not initialized. This is probably due to incorrect object state.");
            if (this.heartbeat != null)
                this.heartbeat.StopTimer();
        }

        /// <summary>
        /// Method takes application instance out of service at the eureka service.
        /// </summary>
        public void TakeInstanceOutOfService()
        {
            if (this.eureka != null)
                Task.Run(() => this.eureka.TakeInstanceOutOfService(DataCenterMetadata)).Wait();
            else
                throw new Exception("Current Eureka client is not initialized. This is probably due to incorrect object state.");
            if (this.heartbeat != null)
                this.heartbeat.StopTimer();
        }

        /// <summary>
        /// Method takes application instance back to service at the eureka service.
        /// </summary>
        public void PutInstanceToService()
        {
            if (this.eureka != null)
                Task.Run(() => this.eureka.PutInstanceToService(DataCenterMetadata)).Wait();
            else
                throw new Exception("Current Eureka client is not initialized. This is probably due to incorrect object state.");
            if (this.heartbeat != null)
                this.heartbeat.StartTimer();
        }

        /// <summary>
        /// Method disposes object resources.
        /// </summary>
        public void Dispose()
        {
            if (this.heartbeat != null)
                this.heartbeat.Dispose();
        }
    }
}
