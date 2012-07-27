namespace Andy.TinyOS
{
    public class Page
    {
    	private readonly Ram _ram;

    	public uint PageNumber { get; set; }
		public uint Size { get; set; }
		public uint FrameNumber { get; set; }

    	public Page(Ram ram)
    	{
    		_ram = ram;
    	}
		
    	public VirtualAddress VirtualAddress
    	{
    		get { return _ram.ToVirtualAddress(this); }
    	}
    }
}