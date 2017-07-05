namespace Andy.TinyOS
{
	public enum OpCodeFlag : byte
	{
		None = 0,
		Register = 1,
		MemoryAddress = 2,
		Constant = 3,
	}
}