using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AmazonEchoApi {
	public class TaskItem {
		[JsonProperty("itemId")]
		public string Id { get; set; }
		public string CustomerId { get; set; }
		public bool Deleted { get; set; }
		public string Text { get; set; }
		public bool Complete { get; set; }
		public int Version { get; set; }
		public DateTime Created { get; set; }
		public DateTime? LastUpdatedDate { get; set; }
	}

	public class TaskItemCollection {
		public List<TaskItem> Values { get; set; }
	}
}
