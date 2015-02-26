using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AmazonEchoApi {
	public class BluetoothState {
		public string DeviceSerialNumber { get; set; }
		public string DeviceType { get; set; }
		[JsonProperty("online")]
		public bool IsOnline { get; set; }
		//public int? SconeBattery { get; set; }
		public bool SconePaired { get; set; }
		public string SoftwareVersion { get; set; }
		public string StreamingState { get; set; }
	}
}
