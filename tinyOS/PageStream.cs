using System;
using System.IO;
using System.Linq;

namespace tinyOS
{
	public class PageStream : Stream
	{
		private readonly Ram _ram;
		private readonly PageInfo _pageSet;
		private Page _page;

		public PageStream(Ram ram, PageInfo pageSet)
		{
			if (ram == null)
				throw new ArgumentNullException("ram");

			if (pageSet == null)
				throw new ArgumentNullException("pageSet");

			_ram = ram;
			_pageSet = pageSet;
			_page = pageSet.Pages.FirstOrDefault();
		}

		public override void Flush()
		{
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			switch (origin)
			{
				case SeekOrigin.Begin:
					Position = Math.Max(offset, 0L);
					break;

				case SeekOrigin.Current:
					Position += offset;
					break;

				case SeekOrigin.End:
					Position = Math.Min(Length - offset, 0L);
					break;

				default:
					throw new ArgumentOutOfRangeException("origin");
			}

			return Position;
		}

		public override void SetLength(long value)
		{
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			if (count <= 0)
				return 0;

			var bytesRead = 0U;
			var bytesRemaining = (uint)count;

			var addr = new VirtualAddress((uint) (_pageSet.Offset + Position));
			if (_page == null || _page.PageNumber != addr.PageNumber )
				_page = _pageSet.Pages.Single(x => x.PageNumber == addr.PageNumber);

			MemoryStream current = _ram.GetStream(_page);
			current.Seek(addr.Offset, SeekOrigin.Begin);
			var remaining = _page.Size - addr.Offset;

			while (bytesRemaining > 0)
			{
				var read = (uint )current.Read(buffer, offset, (int) Math.Min(remaining, bytesRemaining));
				bytesRemaining -= read;
				bytesRead += read;
				offset += (int )read;
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
			if ( count <= 0 )
				return;

			var addr = new VirtualAddress((uint)(_pageSet.Offset + Position));
			if ( _page == null || _page.PageNumber != addr.PageNumber )
				_page = _pageSet.Find(addr.PageNumber);

			MemoryStream current = _ram.GetStream(_page);
			current.Seek(addr.Offset, SeekOrigin.Begin);
			var remaining = _page.Size - addr.Offset;

			while ( offset < count )
			{
				var write = Math.Min(remaining, count - offset);
				current.Write(buffer, offset, (int) write);
				offset += (int)write;
				Position += write;

				_page = _pageSet.Next(_page);
				if ( _page == null )
					return;

				current = _ram.GetStream(_page);
				remaining = _page.Size;
			}
		}

		public override bool CanRead
		{
			get { return true; }
		}

		public override bool CanSeek
		{
			get { return true; }
		}

		public override bool CanWrite
		{
			get { return true; }
		}

		public override long Length
		{
			get { return _pageSet.Size; }
		}

		private long _position;

		public override long Position
		{
			get { return _position; }
			set { _position = value; }
		}
	}
}