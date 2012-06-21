using System.IO;

namespace tinyOS
{
	public class Page
	{
		public uint Owner { get; set; }
		public uint PhysicalOffset { get; set; }
		public uint Size { get; set; }

		public Stream Data { get; set; }
	}
}