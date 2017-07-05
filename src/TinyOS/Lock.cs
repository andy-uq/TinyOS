namespace Andy.TinyOS
{
	public class Lock
	{
		public Lock(DeviceId deviceId)
		{
			Handle = deviceId;
		}

		public uint Owner { get; set; }
		public int RefCount { get; set; }

		public DeviceId Handle { get; }
	}
}