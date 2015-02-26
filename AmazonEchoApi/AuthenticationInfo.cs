using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AmazonEchoApi {
	public class AuthenticationInfo {
		public string CustomerEmail { get; set; }
		public bool CanAccessPrimeMusicContent { get; set; }
		[JsonProperty("authenticated")]
		public bool IsAuthentication { get; set; }
		public string CustomerId { get; set; }
		public string CustomerName { get; set; }
	}
}
