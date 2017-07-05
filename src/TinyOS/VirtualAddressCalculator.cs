using System;

namespace Andy.TinyOS
{
	public class VirtualAddressCalculator
	{
		public VirtualAddressCalculator(uint frameSize)
		{
			PageShift = 0;

			frameSize >>= 1;
			while (frameSize > 0)
			{
				PageShift++;
				OffsetMask = (ushort) (OffsetMask << 1 | 1);
				frameSize >>= 1;
			}
		}

		public int PageShift { get; }

		public ushort OffsetMask { get; }

		public int PageSize => 1 << PageShift;

		public int MaxPages => 1 << (32 - PageShift);

		public uint Address(VirtualAddress vAddr)
		{
			return (vAddr.PageNumber << PageShift) | vAddr.Offset;
		}

		public VirtualAddress New(int offset, int pageNumber)
		{
			if (offset < 0 || offset > OffsetMask)
				throw new ArgumentOutOfRangeException(nameof(offset), offset, "Offset must be greater than zero and less than " + OffsetMask);

			if (pageNumber < 0)
				throw new ArgumentOutOfRangeException(nameof(pageNumber), offset, "Page PageNumber must be greater than zero");

			return new VirtualAddress(this, offset, pageNumber);
		}

		public VirtualAddress New(uint address)
		{
			var pageNumber = address >> PageShift;
			var offset = address & OffsetMask;

			return new VirtualAddress(this, (int) offset, (int) pageNumber);
		}
	}
}