namespace Andy.TinyOS
{
	public enum DeviceId : uint
	{
		Lock1 = Devices.Locks + 1,
		Lock2,
		Lock3,
		Lock4,
		Lock5,
		Lock6,
		Lock7,
		Lock8,
		Lock9,
		Lock10,

		Event1 = Devices.Events + 1,
		Event2,
		Event3,
		Event4,
		Event5,
		Event6,
		Event7,
		Event8,
		Event9,
		Event10,

		Terminal = Devices.Terminal,
		Sleep = Devices.Sleep,
	}
}