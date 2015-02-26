using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonEchoApi {
	class MediaStatePayload {

		public string DeviceSerialNumber { get; set; }
		public string DeviceType { get; set; }
		public int? NewMediaVolume { get; set; }
	}
}
