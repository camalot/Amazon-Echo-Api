using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonEchoApi {
	public enum NotificationTypes {
		Alarm,
		Timer
	}

	public class NotificationItem {
		public DateTime AlarmTime { get; set; }
		public string DeviceSerialNumber { get; set; }
		public string DeviceType { get; set; }
		public string Id { get; set; }
		public string NotificationIndex { get; set; }
		public int RemainingTime { get; set; }
		public string Status { get; set; }
		public int TriggerTime { get; set; }
		public NotificationTypes Type { get; set; }
		public string Version { get; set; }
	}
}
