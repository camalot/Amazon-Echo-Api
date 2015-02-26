using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AmazonEchoApi {
	public class EchoDeviceCollection {
		public EchoDeviceCollection() {
			Devices = new List<EchoDevice>();
		}

		public IEnumerable<EchoDevice> Devices { get; set; }
	}

	public class EchoDevice {
		[JsonProperty("deviceAccountId")]
		public string AccountId { get; set; }
		[JsonProperty("accountName")]
		public string Name { get; set; }
		[JsonProperty("deviceOwnerCustomerId")]
		public string CustomerId { get; set; }
		[JsonProperty("deviceType")]
		public string Type { get; set; }
		public bool Online { get; set; }
		public string PostalCode { get; set; }
		public string RegistrationId { get; set; }
		public string SerialNumber { get; set; }
		public string SoftwareVersion { get; set; }
	}
}
