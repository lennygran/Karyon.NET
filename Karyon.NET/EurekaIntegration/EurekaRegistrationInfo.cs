using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Karyon.EurekaIntegration
{
    [Serializable()]
    [DataContract()]
    public class EurekaRegistrationInfo
    {
        [DataMember(Name = "instance", Order = 1)]
        public InstanceInfo Instance { get; set; }

        public EurekaRegistrationInfo()
        {
            this.Instance = new InstanceInfo();
        }
    }

    [Serializable()]
    [DataContract()]
    public class InstanceInfo
    {
        [DataMember(Name = "hostName", Order = 1)]
        public string HostName { get; set; }
        [DataMember(Name = "app", Order = 2)]
        public string App { get; set; }
        [DataMember(Name = "ipAddr", Order = 3)]
        public string IPAddr { get; set; }
        [DataMember(Name = "vipAddress", Order = 4)]
        public string VipAddress { get; set; }
        [DataMember(Name = "secureVipAddress", Order = 5)]
        public string SecureVipAddress { get; set; }
        [DataMember(Name = "status", Order = 6)]
        [JsonConverter(typeof(StringEnumConverter))]
        public InstanceStatus Status { get; set; }
        [DataMember(Name = "port", Order = 7)]
        public int Port { get; set; }
        [DataMember(Name = "securePort", Order = 8)]
        public int SecurePort { get; set; }
        [DataMember(Name = "dataCenterInfo", Order = 9)]
        public DataCenterInfo DataCenterInfo { get; set; }
        [DataMember(Name = "leaseInfo", Order = 10)]
        public LeaseInfo LeaseInfo { get; set; }
        //[DataMember(Name = "metadata", Order = 11)]
        //public AppMetadataType Metadata { get; set; }

        public InstanceInfo()
        {
            this.HostName = "";
            this.App = "";
            this.IPAddr = "";
            this.VipAddress = "";
            this.SecureVipAddress = "";
            this.Status = InstanceStatus.UP; 
            this.DataCenterInfo = new DataCenterInfo();
            this.LeaseInfo = new LeaseInfo();
            //this.Metadata = new AppMetadataType();
        }
    }

    [Serializable()]
    [DataContract()]
    public class DataCenterInfo
    {
        [DataMember(Name = "name", Order=1)]
        [JsonConverter(typeof(StringEnumConverter))]
        public DataCenterType Name { get; set; }
        [DataMember(Name = "metadata", Order=2)]
        public DataCenterMetadata Metadata { get; set; }

        public DataCenterInfo()
        {
            this.Name = DataCenterType.Amazon; 
            this.Metadata = new DataCenterMetadata();
        }
    }

    [Serializable()]
    [DataContract()]
    public enum InstanceStatus
    {
        UP,
        DOWN,
        STARTING
    }

    [Serializable()]
    [DataContract()]
    public enum DataCenterType
    {
        Amazon,
        MyOwn
    }

    /// <summary>
    /// Data center related metadata being submitted to the Eureka service. 
    /// Generally information is related to Amazon EC2 data center, but other data centers can use it.
    /// </summary>
    /// <remarks>Additional info can be found at http://docs.aws.amazon.com/AWSEC2/latest/UserGuide/AESDG-chapter-instancedata.html. </remarks>
    [Serializable()]
    [DataContract()]
    public class DataCenterMetadata
    {
        /// <summary>
        /// If you started more than one instance at the same time, this value indicates the order in which the instance was launched. The value of the first instance launched is 0.
        /// </summary>
        [DataMember(Name = "ami-launch-index")]
        public string AmiLaunchIndex { get; set; }
        /// <summary>
        /// Gets or sets the private DNS hostname of the instance. In cases where multiple network interfaces are present, this refers to the eth0 device (the device for which the device number is 0).
        /// </summary>
        [DataMember(Name = "local-hostname")]
        public string LocalHostName { get; set; }
        /// <summary>
        /// Gets or sets the Availability Zone in which the instance launched.
        /// </summary>
        [DataMember(Name = "availability-zone")]
        public string AvailabilityZone { get; set; }
        /// <summary>
        /// Gets or sets the ID of this instance.
        /// </summary>
        [DataMember(Name = "instance-id")]
        public string InstanceId { get; set; }
        /// <summary>
        /// Gets or sets the public IP address. If an elastic IP address is associated with the instance, the value returned is the elastic IP address.
        /// </summary>
        [DataMember(Name = "public-ipv4")]
        public string PublicIPv4 { get; set; }
        /// <summary>
        /// Gets or sets the instance's public DNS. If the instance is in a VPC, this category is only returned if the enableDnsHostnames attribute is set to true.
        /// </summary>
        [DataMember(Name = "public-hostname")]
        public string PublicHostName { get; set; }
        /// <summary>
        /// Gets or sets the path to the AMI's manifest file in Amazon S3. If you used an EBS-backed AMI to launch the instance, the returned result is unknown.
        /// </summary>
        [DataMember(Name = "ami-manifest-path")]
        public string AmiManifestPath { get; set; }
        /// <summary>
        /// Gets or sets the private IP address of the instance. In cases where multiple network interfaces are present, this refers to the eth0 device (the device for which the device number is 0).
        /// </summary>
        [DataMember(Name = "local-ipv4")]
        public string LocalIPv4 { get; set; }
        /// <summary>
        /// Gets or sets the private hostname of the instance. In cases where multiple network interfaces are present, this refers to the eth0 device (the device for which the device number is 0).
        /// </summary>
        [DataMember(Name = "hostname")]
        public string HostName { get; set; }
        /// <summary>
        /// Gets or sets the AMI ID used to launch the instance.
        /// </summary>
        [DataMember(Name = "ami-id")]
        public string AmiId { get; set; }
        /// <summary>
        /// Gets or sets the type of instance.
        /// </summary>
        [DataMember(Name = "instance-type")]
        public string InstanceType { get; set; }
        public DataCenterType DataCenterName { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public DataCenterMetadata()
        {
            this.AmiLaunchIndex = "";
            this.LocalHostName = "";
            this.AvailabilityZone = "";
            this.InstanceId = "";
            this.PublicIPv4 = "";
            this.PublicHostName = "";
            this.AmiManifestPath = "";
            this.LocalIPv4 = "";
            this.HostName = "";
            this.AmiId = "";
            this.InstanceType = "";
            this.DataCenterName = DataCenterType.Amazon;
        }
    }

    [Serializable()]
    [DataContract()]
    public class AppMetadataType : List<string>
    {
        public AppMetadataType()
            : base()
        {
        }
    }

    [Serializable()]
    [DataContract()]
    public class LeaseInfo
    {
        [DataMember(Name = "evictionDurationInSecs")]
        public int EvictionDurationInSecs { get; set; }

        public LeaseInfo()
        {
            this.EvictionDurationInSecs = 90;
        }
    }
}