using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonEchoApi {
	public enum CardTypes {
		ToDoCard,
		WeatherCard,
	}
	public class CardItem {
		public CardTypes CardType { get; set; }
		public DateTime CreationTimestamp { get; set; }
		public string CustomerId { get; set; }
		public string RegisteredCustomerId { get; set; }
		public string Id { get; set; }
		public EchoDevice SourceDevice { get; set; }
	}
}
