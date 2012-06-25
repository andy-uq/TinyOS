using System;
using System.Linq;

namespace tinyOS
{
	public class Ram
	{
		private byte[] _ram;
		private Page[] _pageTable;
		public int Size
		{
			get { return _ram.Length; }
		}

		public int FrameSize { get; private set; }

		public Ram(int size, int frameSize)
		{
			_ram = new byte[size];
			_pageTable = Enumerable.Range(0, size / frameSize).Select(x => new Page()).ToArray();

			FrameSize = frameSize;
		}

		public uint ToPhysicalAddress(VirtualAddress vaddr)
		{
			var page = _pageTable[vaddr.PageNumber];
			return page.FrameAddress + vaddr.Offset;
		}
		
		private class Page
		{
			public uint ProcessId { get; set; }
			public bool Live { get; set; }
			public bool Pinned { get; set; }
			public uint FrameAddress { get; set; }
		}
	}

	public struct VirtualAddress
	{
		private const int PAGE_SHIFT = 12;
		private const ushort OFFSET_MASK = 0xfff;

		public ushort Offset { get; private set; }
		public uint PageNumber { get; private set; }

		public uint Address
		{
			get { return (PageNumber << PAGE_SHIFT) | Offset; }
		}

		public VirtualAddress(int offset, int pageNumber) : this()
		{
			if (offset < 0 || offset > OFFSET_MASK)
				throw new ArgumentOutOfRangeException("offset", offset, "Offset must be greater than zero and less than " + OFFSET_MASK);

			if (pageNumber < 0)
				throw new ArgumentOutOfRangeException("pageNumber", offset, "Page Number must be greater than zero");

			Offset = (ushort )offset;
			PageNumber = (uint) pageNumber;
		}

		public VirtualAddress(uint address) : this()
		{
			PageNumber = address >> PAGE_SHIFT;
			Offset = (ushort) (address & OFFSET_MASK);
		}

		public override string ToString()
		{
			return string.Format("[0x{0:x8}] Page: {1:n}, Offset {2}", Address, PageNumber, Offset);
		}
	}

	public static class VirtualAddressLookup
	{

	}

	public class TranslationLookasideBuffer
	{
		
	}
}