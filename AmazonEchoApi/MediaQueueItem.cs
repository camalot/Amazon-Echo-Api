using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AmazonEchoApi {
	public class MediaQueueItem {
		public string Album { get; set; }
		public string Artist { get; set; }
		[JsonProperty("asin")]
		public string ASIN { get; set; }
		public string CardImageUrl { get; set; }
		public string ContentId { get; set; }
		public string ContentType { get; set; }
		public int DurationSeconds { get; set; }
		[JsonProperty("feedbackDisabled")]
		public bool IsFeedbackDisabled { get; set; }
		public string HistoricalId { get; set; }
		[JsonProperty("imageURL")]
		public string ImageUrl { get; set; }
		public int Index { get; set; }
		public bool IsAd { get; set; }
		public bool IsDisliked { get; set; }
		public bool IsFreeWithPrime { get; set; }
		public bool IsLiked { get; set; }
		public string ProgramId { get; set; }
		public string ProgramName { get; set; }
		public string ProviderName { get; set; }
		public string ProviderId { get; set; }
		public string QueueId { get; set; }
		public string RadioStationCallSign { get; set; }
		public string RadioStationId { get; set; }
		public string RadioStationLocation { get; set; }
		public string RadioStationName { get; set; }
		public string RadioStationSlogan { get; set; }
		public string ReferenceId { get; set; }
		public string Service { get; set; }
		public DateTime? StartTime { get; set; }
		public string Title { get; set; }
		public string TrackStatus { get; set; }
	}
}
