using System;

namespace tinyOS
{
	public class PageInfo : IEquatable<PageInfo>
	{
		public PageInfo(Page page)
		{
			Owner = page.Owner;
			Offset = page.PhysicalOffset;
			Size = page.Size;
		}

		public uint Owner { get; private set; }
		public uint Offset { get; private set; }
		public uint Size { get; private set; }

		public bool Equals(PageInfo other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return other.Owner == Owner && other.Offset == Offset && other.Size == Size;
		}

		public override string ToString()
		{
			return string.Format("[0x{0:x4}] ({1:n0} bytes)", Offset, Size);
		}
	}
}