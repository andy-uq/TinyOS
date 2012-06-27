using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace tinyOS
{
 	public class Ram
	{
		private readonly byte[] _ram;
		private readonly List<Frame>[] _pageTable;
		
        public int Size
		{
			get { return _ram.Length; }
		}

 	    public uint FrameSize { get; private set; }
 	    public ushort FrameCount { get { return (ushort) _pageTable.Length; } }

        public Ram(byte[] ram, uint frameSize)
        {
            FrameSize = frameSize;
            _ram = ram;
            _pageTable = Enumerable.Range(0, (int) (ram.Length/frameSize))
                .Select(x => CreateFrame(0, (uint) x))
                .Select(x => new List<Frame> {x})
                .ToArray();
        }

 	    public Ram(uint size, uint frameSize) : this(new byte[size], frameSize)
		{
		}

	    private Frame CreateFrame(uint processId, uint frameNumber)
	    {
	        return new Frame
	                   {
                           FrameNumber = frameNumber,
	                       ProcessId = processId, 
                           FrameAddress = frameNumber * FrameSize,
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
                    from frame in LiveFrames
                    where frame.ProcessId == 0
                    select frame
                ).FirstOrDefault();

            if (firstFreeFrame == null)
                return null;

            var page = pcb.PageTable.CreatePage();
            firstFreeFrame.ProcessId = pcb.Id;
            firstFreeFrame.PageNumber = page.PageNumber;
            page.Size = FrameSize;
            page.FrameNumber = firstFreeFrame.FrameNumber;

            return page;
        }
		
        public void Free(Page page)
        {
            var frameList = _pageTable[page.FrameNumber];
            var frameIndex = frameList.FindIndex(x => x.PageNumber == page.PageNumber);
            if (frameIndex == -1)
                return;

            if (frameIndex == 0)
            {
                var frame = frameList[0];
                frame.PageNumber = 0;
                frame.ProcessId = 0;
            }
            else
            {
                frameList.RemoveAt(frameIndex);
            }
        }

	    private IEnumerable<Frame> LiveFrames
	    {
	        get { return _pageTable.Select(frameList => frameList[0]); }
	    }

        public MemoryStream GetStream(Page page)
        {
            return GetStream(page.FrameNumber);
        }

        public MemoryStream GetStream(uint frameNumber)
        {
            return new MemoryStream(_ram, (int) (frameNumber * FrameSize), (int) FrameSize, true);
        }
		
		private class Frame
		{
            public uint FrameNumber { get; set; }
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
				throw new ArgumentOutOfRangeException("pageNumber", offset, "Page PageNumber must be greater than zero");

			Offset = (ushort )offset;
			PageNumber = (uint) pageNumber;
		}

		public VirtualAddress(uint address) : this()
		{
			PageNumber = address >> PAGE_SHIFT;
			Offset = (ushort) (address & OFFSET_MASK);
		}

        public static VirtualAddress operator +(VirtualAddress address, uint offset)
        {
            return new VirtualAddress(address.Address + offset);
        }

        public static VirtualAddress operator -(VirtualAddress address, uint offset)
        {
            return new VirtualAddress(address.Address - offset);
        }

	    public override string ToString()
		{
			return string.Format("[0x{0:x8}] Page: {1:n0}, Offset {2}", Address, PageNumber, Offset);
		}

        public static uint ToAddress(Page page, int offset = 0)
        {
            var virtualAddress = new VirtualAddress(offset, (int) page.PageNumber);
            return virtualAddress.Address;
        }
	}

	public static class VirtualAddressLookup
	{

	}

	public class TranslationLookasideBuffer
	{
		
	}
}