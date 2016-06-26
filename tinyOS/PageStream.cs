using System;
using System.IO;
using System.Linq;

namespace Andy.TinyOS
{
	public class PageStream : Stream
	{
		private readonly PageInfo _pageSet;
		private readonly Ram _ram;
		private Page _page;

		public PageStream(Ram ram, PageInfo pageSet)
		{
			if (ram == null)
				throw new ArgumentNullException(nameof(ram));

			if (pageSet == null)
				throw new ArgumentNullException(nameof(pageSet));

			_ram = ram;
			_pageSet = pageSet;
			_page = pageSet.Pages.FirstOrDefault();
		}

		public override bool CanRead => true;

		public override bool CanSeek => true;

		public override bool CanWrite => true;

		public override long Length => _pageSet.Size;

		public override long Position { get; set; }

		public override void Flush()
		{
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			long position = Position;
			switch (origin)
			{
				case SeekOrigin.Begin:
					position = Math.Max(offset, 0L);
					break;

				case SeekOrigin.Current:
					position += offset;
					break;

				case SeekOrigin.End:
					position = Math.Min(Length - offset, 0L);
					break;

				default:
					throw new ArgumentOutOfRangeException(nameof(origin));
			}
			
			if (position < 0L)
				throw new InvalidOperationException("Attempt to seek before start of stream");

			if (position > Length)
				throw new InvalidOperationException("Attempt to seek beyond length of stream");

			return Position = position;
		}

		public override void SetLength(long value)
		{
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			if (count <= 0)
				return 0;

			uint bytesRead = 0U;
			var bytesRemaining = (uint) count;

			var addr = _ram.VirtualAddressCalculator.New((uint) (_pageSet.Offset + Position));
			if (_page == null || _page.PageNumber != addr.PageNumber)
				_page = _pageSet.Pages.Single(x => x.PageNumber == addr.PageNumber);

			MemoryStream current = _ram.GetStream(_page);
			current.Seek(addr.Offset, SeekOrigin.Begin);
			uint remaining = _page.Size - addr.Offset;

			while (bytesRemaining > 0)
			{
				var read = (uint) current.Read(buffer, offset, (int) Math.Min(remaining, bytesRemaining));
				bytesRemaining -= read;
				bytesRead += read;
				offset += (int) read;
				Position += read;

				_page = _pageSet.Next(_page);
				if (_page == null)
					break;

				current = _ram.GetStream(_page);
				remaining = _page.Size;
			}

			return (int) bytesRead;
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			if (count <= 0)
				return;

			var addr = _ram.VirtualAddressCalculator.New((uint) (_pageSet.Offset + Position));
			if (_page == null || _page.PageNumber != addr.PageNumber)
			{
				_page = _pageSet.Find(addr.PageNumber);
			}

			MemoryStream current = _ram.GetStream(_page);
			current.Seek(addr.Offset, SeekOrigin.Begin);
			uint remaining = _page.Size - addr.Offset;

			while (offset < count)
			{
				long write = Math.Min(remaining, count - offset);
				current.Write(buffer, offset, (int) write);
				offset += (int) write;
				Position += write;

				_page = _pageSet.Next(_page);
				if (_page == null)
					break;

				current = _ram.GetStream(_page);
				remaining = _page.Size;
			}

			if ( offset == count )
			{
				return;
			}

			throw new InvalidOperationException("Attempt to write past stream boundary");
		}
	}
}