using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Karyon.EurekaIntegration
{
    internal class HeartbeatObserver : IDisposable
    {
        private System.Timers.Timer timer;
        private DataCenterMetadata DataCenterMetadata;
        IEurekaClient eureka;
        private IEnumerable<string> EurekaServiceUrls;

        /// <summary>
        /// External function performing additional validation for the service health. 
        /// Function must return boolean value indicating the service health. 
        /// If FALSE value returned, then no HEALTHY heartbeat is being sent.
        /// </summary>
        public Func<bool> OnHealthCheck;

        public HeartbeatObserver(IEurekaClient eureka, DataCenterMetadata dcMetadata, IEnumerable<string> eurekaServiceUrls)
        {
            this.DataCenterMetadata = dcMetadata;
            this.eureka = eureka;
            this.EurekaServiceUrls = eurekaServiceUrls;
        }

        public void InitializeAndStartTimer()
        {
            timer = new System.Timers.Timer();
            timer.Elapsed += new System.Timers.ElapsedEventHandler(TimerWorker);
            timer.Interval = 30000;
            timer.Enabled = true;
            timer.AutoReset = false;
            timer.Start();
        }

        private void TimerWorker(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                if (this.OnHealthCheck != null)
                {
                    if (!this.OnHealthCheck())
                    {
                        timer.Start();
                        return;
                    }
                }
                EurekaClientExecutor.Wrap(this.eureka).ExecuteWithRetry(() => this.eureka.SendHeartbeat(DataCenterMetadata), this.EurekaServiceUrls);
                timer.Start();
            }
            catch (Exception ex)
            {
                Trace.TraceError("Exception: " + ex.ToString());
            }
        }

        public void StopTimer()
        {
            if (timer != null)
                timer.Stop();
        }

        public void StartTimer()
        {
            if (timer != null)
                timer.Start();
        }

        public void Dispose()
        {
            if (timer != null)
                timer.Dispose();
        }
    }
}
