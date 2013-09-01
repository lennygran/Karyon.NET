using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Diagnostics;

namespace Karyon.EurekaIntegration
{
    internal class EurekaClient : IEurekaClient
    {
        public string EurekaServiceUrl { get; set; }
        public string ApplicationName { get; set; }
        public int ApplicationPort { get; set; }
        public int ApplicationSecurePort { get; set; }

        private string EurekaServiceRoot
        {
            get
            {
                //EXAMPLE: "http://ec2-174-129-161-75.compute-1.amazonaws.com/eureka/v2/apps/DemoService"

                string tmp = this.EurekaServiceUrl.EndsWith("/") ? this.EurekaServiceUrl : this.EurekaServiceUrl + "/";
                return string.Join("", new string[] { tmp, "v2/apps/", this.ApplicationName });
            }
        }
        private IHttpClientFactory httpClientFactory;

        public EurekaClient()
            : this(new HttpClientFactory())
        {
        }

        internal EurekaClient(IHttpClientFactory httpClientFactory)
        {
            this.EurekaServiceUrl = "";
            this.ApplicationPort = 80;
            this.ApplicationName = "";
            this.httpClientFactory = httpClientFactory;
        }

        //Register new application instance 
        //POST /eureka/v2/apps/appID 
        //Input: JSON/XML payload HTTP Code: 204 on success 
        public async Task<bool> Register(DataCenterMetadata dcData)
        {
            EurekaRegistrationInfo instanceData = new EurekaRegistrationInfo();
            instanceData.Instance.DataCenterInfo.Name = dcData.DataCenterName;
            instanceData.Instance.DataCenterInfo.Metadata = dcData;
            instanceData.Instance.HostName = dcData.HostName;// "ec2-50-16-138-165.compute-1.amazonaws.com";
            instanceData.Instance.App = this.ApplicationName;
            instanceData.Instance.Port = this.ApplicationPort;
            instanceData.Instance.IPAddr = dcData.LocalIPv4;
            instanceData.Instance.VipAddress = dcData.PublicIPv4;// "50.16.138.165";
            instanceData.Instance.SecurePort = this.ApplicationSecurePort;
            if (this.ApplicationSecurePort > 0)
                instanceData.Instance.SecureVipAddress = dcData.PublicIPv4;

            HttpClient client = null;
            string url = this.EurekaServiceRoot;
            try
            {
                System.Net.Http.Formatting.MediaTypeFormatter frm = new System.Net.Http.Formatting.JsonMediaTypeFormatter();
                client = this.httpClientFactory.CreateInstance();
                HttpResponseMessage response = await client.PostAsync(url, instanceData, frm, "application/json");
                //response.EnsureSuccessStatusCode();
                if (response.StatusCode != System.Net.HttpStatusCode.NoContent)
                {
                    string tmp = await response.Content.ReadAsStringAsync();
                    Trace.TraceError("Error on Register: " + tmp);
                    return false;
                }
                else
                    Trace.TraceInformation("Registration completed. Status: " + response.StatusCode.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return true;
        }

        //De-register application instance 
        //DELETE /eureka/v2/apps/appID/instanceID 
        //HTTP Code: 200 on success 
        public async Task<bool> Unregister(DataCenterMetadata dcData)
        {
            string url = this.EurekaServiceRoot + "/" + dcData.InstanceId;
            try
            {
                HttpClient client = this.httpClientFactory.CreateInstance();
                HttpResponseMessage response = await client.DeleteAsync(url);
                //response.EnsureSuccessStatusCode();
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    string tmp = await response.Content.ReadAsStringAsync();
                    Trace.TraceError("Exception on Unregister: " + tmp);
                    return false;
                }
                else
                    Trace.TraceInformation("Unregistered successfully.");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return true;
        }

        //Send application instance heartbeat 
        //PUT /eureka/v2/apps/appID/instanceID 
        //HTTP Code:
        // * 200 on success
        // * 404 if instanceID doesn’t exist 
        public async Task<bool> SendHeartbeat(DataCenterMetadata dcData)
        {
            string url = this.EurekaServiceRoot + "/" + dcData.InstanceId;
            try
            {
                HttpClient client = this.httpClientFactory.CreateInstance();
                System.Net.Http.Formatting.MediaTypeFormatter frm = new System.Net.Http.Formatting.JsonMediaTypeFormatter();
                HttpResponseMessage response = await client.PutAsync(url, "", frm, "application/json");
                //response.EnsureSuccessStatusCode();
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    string tmp = await response.Content.ReadAsStringAsync();
                    Trace.TraceError("Error on Heartbeat: " + tmp);
                    return false;
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    Trace.TraceWarning("Heartbeat failed. Instance does not exist.");
                    return false;
                }
                else
                    Trace.TraceInformation("Heartbeat has been successfully submitted.");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return true;
        }

        //Take instance out of service 
        //PUT /eureka/v2/apps/appID/instanceID/status?value=OUT_OF_SERVICE
        //HTTP Code:
        // * 200 on success
        // * 500 on failure 
        public async Task<bool> TakeInstanceOutOfService(DataCenterMetadata dcData)
        {
            string url = this.EurekaServiceRoot + "/" + dcData.InstanceId + "/status?value=OUT_OF_SERVICE";
            try
            {
                HttpClient client = this.httpClientFactory.CreateInstance();
                System.Net.Http.Formatting.MediaTypeFormatter frm = new System.Net.Http.Formatting.JsonMediaTypeFormatter();
                HttpResponseMessage response = await client.PutAsync(url, "", frm, "application/json");
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    string tmp = await response.Content.ReadAsStringAsync();
                    Trace.TraceError("Error on TakeInstanceOutOfService: " + tmp);
                    return false;
                }
                else
                    Trace.TraceInformation("TakeInstanceOutOfService has been successfully submitted.");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return true;
        }

        //Put instance back into service 
        //PUT /eureka/v2/apps/appID/instanceID/status?value=UP 
        //HTTP Code:
        // * 200 on success
        // * 500 on failure 
        public async Task<bool> PutInstanceToService(DataCenterMetadata dcData)
        {
            string url = this.EurekaServiceRoot + "/" + dcData.InstanceId + "/status?value=UP";
            try
            {
                HttpClient client = this.httpClientFactory.CreateInstance();
                System.Net.Http.Formatting.MediaTypeFormatter frm = new System.Net.Http.Formatting.JsonMediaTypeFormatter();
                HttpResponseMessage response = await client.PutAsync(url, "", frm, "application/json");
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    string tmp = await response.Content.ReadAsStringAsync();
                    Trace.TraceError("Error on TakeInstanceDown: " + tmp);
                    return false;
                }
                else
                    Trace.TraceInformation("TakeInstanceDown has been successfully submitted.");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return true;
        }

        //Query for all instances 
        //GET /eureka/v2/apps 
        //HTTP Code: 200 on success Output: JSON/XML 

        //Query for all appID instances 
        //GET /eureka/v2/apps/appID 
        //HTTP Code: 200 on success Output: JSON/XML 

        //Query for a specific appID/instanceID 
        //GET /eureka/v2/apps/appID/instanceID 
        //HTTP Code: 200 on success Output: JSON/XML 

        //Query for a specific instanceID 
        //GET /eureka/v2/instances/instanceID 
        //HTTP Code: 200 on success Output: JSON/XML 
    }
}



