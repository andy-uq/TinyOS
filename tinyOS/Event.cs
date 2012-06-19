namespace tinyOS
{
	public class Event
	{
		public Event(DeviceId deviceId)
		{
			Handle = deviceId;
		}

		public DeviceId Handle { get; set; }
	}
}