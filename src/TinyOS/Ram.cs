using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Andy.TinyOS
{
	public class Ram
	{
		private readonly List<Frame>[] _pageTable;
		private readonly byte[] _ram;
		private Frame[] _shared;

		public Ram(byte[] ram, uint frameSize)
		{
			VirtualAddressCalculator = new VirtualAddressCalculator(frameSize);
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

		public VirtualAddressCalculator VirtualAddressCalculator { get; }

		public int Size => _ram.Length;

		public uint FrameSize { get; }
		public ushort FrameCount => (ushort) _pageTable.Length;

		private IEnumerable<Frame> LiveFrames
		{
			get { return _pageTable.Select(frameList => frameList[0]); }
		}

		public uint AllocateShared(uint size)
		{
			if (size == 0)
				throw new ArgumentException("Size must be greater than zero");

			_shared = Enumerable
				.Range(1, (int) (size/FrameSize))
				.Select(CreateSharedFrame)
				.ToArray();

			return _shared.Select(x => x.FrameAddress).First();
		}

		private Frame CreateSharedFrame(int i)
		{
			var frame =
				(
					from f in LiveFrames
					where f.ProcessId == 0
					select f
					).FirstOrDefault();

			if (frame == null)
				throw new InvalidOperationException("Cannot allocate space for shared memory");

			frame.Pinned = true;
			frame.PageNumber = (uint) i;
			frame.ProcessId = uint.MaxValue;

			return frame;
		}

		private Frame CreateFrame(uint processId, uint frameNumber)
		{
			return new Frame
			{
				FrameNumber = frameNumber,
				ProcessId = processId,
				FrameAddress = frameNumber*FrameSize,
				Live = true
			};
		}

		public VirtualAddress ToVirtualAddress(Page page)
		{
			return VirtualAddressCalculator.New(0, (int) page.PageNumber);
		}

		public uint ToPhysicalAddress(uint processId, VirtualAddress vaddr)
		{
			var pageNumber = vaddr.PageNumber;
			var frame = LiveFrames.SingleOrDefault(x => x.ProcessId == processId && x.PageNumber == pageNumber);
			if (frame == null)
				throw new InvalidOperationException();

			return frame.FrameAddress + vaddr.Offset;
		}

		public IEnumerable<Page> AllocateFromShared(ProcessContextBlock pcb, uint size)
		{
			long remaining = size;

			var index = 0;
			while (remaining > 0)
			{
				var sharedFrame = _shared[index];
				var page = pcb.PageTable.CreatePage(this);
				page.Size = FrameSize;
				page.FrameNumber = sharedFrame.FrameNumber;
				yield return page;

				remaining -= FrameSize;
				index++;
			}
		}

		public Page Allocate(ProcessContextBlock pcb)
		{
			if (pcb == null)
				throw new ArgumentNullException(nameof(pcb));

			var firstFreeFrame =
				(
					from frame in LiveFrames
					where frame.ProcessId == 0
					select frame
					).FirstOrDefault();

			if (firstFreeFrame == null)
				return null;

			var page = pcb.PageTable.CreatePage(this);
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

		public MemoryStream GetStream(Page page)
		{
			return GetStream(page.FrameNumber);
		}

		public MemoryStream GetStream(uint frameNumber)
		{
			return new MemoryStream(_ram, (int) (frameNumber*FrameSize), (int) FrameSize, true);
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
}