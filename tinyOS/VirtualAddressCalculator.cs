using System;

namespace Andy.TinyOS
{
	public class VirtualAddressCalculator
	{
		private readonly int _pageShift;
		private readonly ushort _offsetMask;

		public VirtualAddressCalculator(uint frameSize)
		{
			_pageShift = 0;

			frameSize >>= 1;
			while ( frameSize > 0 )
			{
				_pageShift++;
				_offsetMask = (ushort)(_offsetMask << 1 | 1);
				frameSize >>= 1;
			}
		}

		public int PageShift
		{
			get { return _pageShift; }
		}

		public ushort OffsetMask
		{
			get { return _offsetMask; }
		}

		public int PageSize
		{
			get { return 1 << PageShift; }
		}

		public int MaxPages
		{
			get { return 1 << (32 - _pageShift); }
		}

		public uint Address(VirtualAddress vAddr)
		{
			return (vAddr.PageNumber << PageShift) | vAddr.Offset;
		}

		public VirtualAddress New(int offset, int pageNumber)
		{
			if (offset < 0 || offset > _offsetMask)
				throw new ArgumentOutOfRangeException("offset", offset, "Offset must be greater than zero and less than " + _offsetMask);

			if (pageNumber < 0)
				throw new ArgumentOutOfRangeException("pageNumber", offset, "Page PageNumber must be greater than zero");

			return new VirtualAddress(this, offset, pageNumber);
		}

		public VirtualAddress New(uint address)
		{
			var pageNumber = address >> PageShift;
			var offset = address & _offsetMask;

			return new VirtualAddress(this, (int )offset, (int )pageNumber);
		}
	}
}