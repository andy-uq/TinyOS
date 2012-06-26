using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace tinyOS.X
{
    public class Page
    {
        public uint Owner { get; set; }
        public uint Number { get; set; }
        public uint FrameNumber { get; set; }
    }

	public class Ram
	{
		private readonly byte[] _ram;
		private readonly List<Frame>[] _pageTable;
		
        public int Size
		{
			get { return _ram.Length; }
		}

		public int FrameSize { get; private set; }

		public Ram(int size, int frameSize)
		{
            FrameSize = frameSize;
            
            _ram = new byte[size];
            _pageTable = Enumerable.Range(0, size / frameSize)
                .Select(x => CreateFrame(0, x))
                .Select(x => new List<Frame> { x })
                .ToArray();
		}

	    private Frame CreateFrame(uint processId, int frameNumber)
	    {
	        return new Frame
	                   {
	                       ProcessId = processId, 
                           FrameAddress = (uint )(frameNumber * FrameSize),
                           Live = true,
	                   };
	    }

	    public uint ToPhysicalAddress(uint processId, VirtualAddress vaddr)
		{
	        uint pageNumber = vaddr.PageNumber;
	        var frame = LiveFrames.SingleOrDefault(x => x.ProcessId == processId && x.PageNumber == pageNumber );
            if (frame == null)
                throw new InvalidOperationException();

			return frame.FrameAddress + vaddr.Offset;
		}

        public Page Allocate(ProcessContextBlock pcb)
        {
            var firstFreeFrame =
                (
                    from f in _pageTable
                    select f.First(x => x.ProcessId == 0)
                ).FirstOrDefault();

            if (firstFreeFrame == null)
                return null;

            var page = pcb.PageTable.Allocate();
            firstFreeFrame.ProcessId = pcb.Id;
            firstFreeFrame.PageNumber = page.Number;

            return page;
        }

	    private IEnumerable<Frame> LiveFrames
	    {
	        get { return _pageTable.Select(frameList => frameList[0]); }
	    }

        public Stream GetStream(Page page)
        {
            return new MemoryStream(_ram, (int)page.FrameNumber * FrameSize, FrameSize, true);
        }
		
		private class Frame
		{
			public uint ProcessId { get; set; }
			public uint PageNumber { get; set; }
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