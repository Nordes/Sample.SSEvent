using System;

namespace Sample.SSEvent.Models {

	public class Notification {
		public int Id { get; set; }
		public string Message { get; set; }
		public NotificationLevel Level { get; set; }
	}
}