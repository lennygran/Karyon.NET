using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using System.Web.Http.SelfHost;
using System.Reflection;

namespace BasicServiceHost
{
    class Program
    {
        static void Main(string[] args)
        {
            Karyon.EurekaIntegration.ApplicationLifecycleController netflixoss = null;
            HttpSelfHostServer server = null;
            bool registered = false;

            System.Diagnostics.Trace.Listeners.Add(new System.Diagnostics.ConsoleTraceListener(true));

            try
            {
                //register application at Eureka service.
                netflixoss = Karyon.EurekaIntegration.ApplicationLifecycleController.CreateInstance();
                registered = netflixoss.RegisterApplicationInstance();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.ToString());
            }

            if (!registered)
            {
                Console.WriteLine("Press Enter to quit.");
                Console.ReadLine();
                if (netflixoss != null)
                    netflixoss.Dispose();
                return;
            }

            try
            {
                Console.WriteLine("Configuring instance services...");
                string listenToPath = string.Format("http://{0}:{1}",
                    (netflixoss.AppConfig.ListenToPublic ? netflixoss.DataCenterMetadata.PublicIPv4 : netflixoss.DataCenterMetadata.LocalIPv4),
                    netflixoss.AppConfig.ApplicationPort.ToString());
                HttpSelfHostConfiguration config = new HttpSelfHostConfiguration(listenToPath);
                AssembliesResolver assemblyResolver = new AssembliesResolver();
                config.Services.Replace(typeof(IAssembliesResolver), assemblyResolver);
                config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;
                config.Routes.MapHttpRoute("API Default", "api/{controller}");
                //add Karyon.NET healthcheck API endpoint as "<root>/healthcheck"
                config.Routes.MapHttpRoute("Healthcheck", "healthcheck", defaults: new { controller = "Healthcheck" });

                server = new HttpSelfHostServer(config);
                server.OpenAsync().Wait();
                Console.WriteLine("");
                Console.WriteLine("Listening to " + config.BaseAddress.ToString() + "; Press Enter to quit.");
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.ToString());
                Console.WriteLine("Press Enter to quit.");
                Console.ReadLine();
            }
            finally
            {
                if (server != null)
                    server.CloseAsync();
                try
                {
                    if (netflixoss != null)
                        netflixoss.UnregisterApplicationInstance();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception: " + ex.Message);
                }
                finally
                {
                    if (netflixoss != null)
                        netflixoss.Dispose();
                }
            }
        }

        private class AssembliesResolver : DefaultAssembliesResolver
        {
            private static List<Assembly> knownScope = new List<Assembly>(new Assembly[] { 
                                                     typeof(DemoService.Controllers.EchoController).Assembly,
                                                     typeof(Karyon.Healthcheck.HealthcheckController).Assembly            
                                                   });

            public override ICollection<Assembly> GetAssemblies()
            {
                ICollection<Assembly> baseAssemblies = base.GetAssemblies();
                return baseAssemblies.Union(knownScope).ToList();
            }
        }
    }
}
