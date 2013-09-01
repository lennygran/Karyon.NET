using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Karyon.EurekaIntegration
{
    internal class EurekaClientExecutor
    {
        public IEurekaClient EurekaClient { get; private set; }

        private EurekaClientExecutor(IEurekaClient eurekaClient)
        {
            this.EurekaClient = eurekaClient;
        }

        public static EurekaClientExecutor Wrap(IEurekaClient eurekaClient)
        {
            return new EurekaClientExecutor(eurekaClient);
        }

        /// <summary>
        /// Method executes EUREKA related function and re-try different EUREKA service URL if fails.
        /// </summary>
        /// <param name="function">Function to execute.</param>
        /// <param name="eurekaServiceUrlsToRetry">List of valid EUREKA service URLs to retry the function with.</param>
        /// <returns>Returns boolean value that indicates a success.</returns>
        public bool ExecuteWithRetry(Func<Task<bool>> function, IEnumerable<string> eurekaServiceUrlsToRetry)
        {
            List<string> eurekaTrialUrls = null;
            if (this.EurekaClient == null)
                throw new Exception("Current Eureka client is not initialized.");

            while (true)
            {
                try
                {
                    Task<bool> task = Task.Run(function);
                    task.Wait();
                    return task.Result;
                }
                catch (Exception ex)
                {
                    Trace.TraceError(string.Format("Exception to execute EUREKA request to {0}: {1}", this.EurekaClient.EurekaServiceUrl, ex.ToString()));
                    if (eurekaTrialUrls == null) //init
                        eurekaTrialUrls = new List<string>();
                    eurekaTrialUrls.Add(this.EurekaClient.EurekaServiceUrl);
                    string foundUrl = eurekaServiceUrlsToRetry.FirstOrDefault(url => !eurekaTrialUrls.Contains(url));
                    if (string.IsNullOrEmpty(foundUrl))
                        return false;
                    else
                        this.EurekaClient.EurekaServiceUrl = foundUrl;
                }
            }
        }

        /// <summary>
        /// Method executes EUREKA related function and re-try number of times if fails.
        /// </summary>
        /// <param name="function">Function to execute.</param>
        /// <param name="retryCount">Number of times to retry the function.</param>
        /// <returns>Returns boolean value that indicates a success.</returns>
        public bool ExecuteWithRetry(Func<Task<bool>> function, int retryCount)
        {
            if (this.EurekaClient == null)
                throw new Exception("Current Eureka client is not initialized.");

            for (int ii = 1; ii <= retryCount; ii++)
            {
                try
                {
                    Task<bool> task = Task.Run(function);
                    task.Wait();
                    return task.Result;
                }
                catch (Exception ex)
                {
                    Trace.TraceError(string.Format("Exception to execute EUREKA request (try {0}): {1}", ii.ToString(), ex.ToString()));
                }
            }
            return false;
        }
    }
}
