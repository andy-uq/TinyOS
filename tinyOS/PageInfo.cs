using System;

namespace tinyOS
{
	public class PageInfo : IEquatable<PageInfo>
	{
		public PageInfo(Page page)
		{
			Owner = page.Owner;
			Offset = page.Offset;
			Size = page.Size;
		}

		public int Owner { get; private set; }
		public uint Offset { get; private set; }
		public uint Size { get; private set; }

		public bool Equals(PageInfo other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return other.Owner == Owner && other.Offset == Offset && other.Size == Size;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof (PageInfo)) return false;
			return Equals((PageInfo) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				int result = Owner;
				result = (result*397) ^ Offset.GetHashCode();
				result = (result*397) ^ Size.GetHashCode();
				return result;
			}
		}

		public override string ToString()
		{
			return string.Format("[0x{0:x4}] ({1:n0} bytes)", Offset, Size);
		}
	}
}