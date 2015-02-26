using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AmazonEchoApi {
	public class MediaState {
		public string ClientId { get; set; }
		public string ContentId { get; set; }
		public string CustomerId { get; set; }
		public string ContentType { get; set; }
		public string CurrentState { get; set; }
		[JsonProperty("imageURL")]
		public string ImageUrl { get; set; }
		public bool IsDisliked { get; set; }
		public bool IsLiked { get; set; }
		[JsonProperty("looping")]
		public bool IsLooping { get; set; }
		[JsonProperty("muted")]
		public bool IsMuted { get; set; }
		public string ProgramId { get; set; }
		public int ProgressSeconds { get; set; }
		public string ProviderId { get; set; }
		public string QueueId { get; set; }
		public IEnumerable<MediaQueueItem> Queue { get; set; }
		public int QueueSize { get; set; }
		public string RadioStationId { get; set; }
		public int RadioVariety { get; set; }
		public string ReferenceId { get; set; }
		public string Service { get; set; }
		[JsonProperty("shuffling")]
		public bool IsShuffling { get; set; }
		public DateTime TimeLastShuffled { get; set; }
		public int Volume { get; set; }

	}
}
