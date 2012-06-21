using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace tinyOS
{
	public class MemoryManager
	{
		private readonly byte[] _data;
		private readonly SortedSet<Page> _pages;
		private readonly SortedSet<Page> _freePages;

		public MemoryManager(uint freeSize, byte[] data)
		{
			_data = data;
			_pages = new SortedSet<Page>(new PageOffsetComparer());
			_freePages = new SortedSet<Page>(new PageSizeComparer()) {new Page {PhysicalOffset = 0, Size = freeSize}};
		}

		public IEnumerable<PageInfo> AllocatedPages
		{
			get { return _pages.Reverse().Select(page => new PageInfo(page)); }
		}

		public IEnumerable<PageInfo> FreePages
		{
			get { return _freePages.Select(page => new PageInfo(page)); }
		}

		public Page Allocate(uint owner, uint size)
		{
			var freeBlock = _freePages.FirstOrDefault(x => size <= x.Size);
			if ( freeBlock == null )
				return null;

			var page = new Page
			{
				Owner = owner,
				PhysicalOffset = freeBlock.PhysicalOffset,
				Size = size,
				Data = new MemoryStream(_data, (int)freeBlock.PhysicalOffset, (int)size, writable: true)
			};

			freeBlock.PhysicalOffset += size;
			freeBlock.Size -= size;

			if (freeBlock.Size == 0)
			{
				_freePages.Remove(freeBlock);
			}

			_pages.Add(page);
			return page;
		}

		public int Translate(PageTable pageTable, uint vAddr)
		{
			uint address = 0;
			foreach ( var page in pageTable )
			{
				if ( vAddr < address + page.Size )
					return (int)(page.PhysicalOffset + vAddr);

				address += page.Size;
				vAddr -= page.Size;
			}

			return -1;
		}

		public void Free(Page page)
		{
			_pages.Remove(page);

			var freePage = CollapseFreePage(page);
			_freePages.Add(freePage);
		}

		public void Compact()
		{
			if ( _pages.Count == 0 )
				return;

			foreach (var freePage in _freePages.ToArray())
			{
				_freePages.Remove(freePage);

				freePage.PhysicalOffset = GetPagesToRight(freePage)
					.Reverse()
					.Aggregate(freePage.PhysicalOffset, (offset, pageToTheRight) => MovePage(pageToTheRight, offset));

				CollapseFreePage(freePage);
			}
		}

		private uint MovePage(Page page, uint newOffset)
		{
			var offset = page.PhysicalOffset;
			page.PhysicalOffset = newOffset;

			return offset;
		}

		private IEnumerable<Page> GetPagesToRight(Page page)
		{
			return _pages.TakeWhile(x => x.PhysicalOffset > page.PhysicalOffset);
		}

		private Page CollapseFreePage(Page page)
		{
			uint offset = page.PhysicalOffset;
			uint size = page.Size;

			Page left;
			while ( (left = _freePages.SingleOrDefault(x => (x.PhysicalOffset + x.Size) == offset)) != null )
			{
				_freePages.Remove(left);
				offset = left.PhysicalOffset;
				size += left.Size;
			}

			Page right;
			while ( (right = _freePages.SingleOrDefault(x => (offset + size) == x.PhysicalOffset)) != null )
			{
				_freePages.Remove(right);
				size += right.Size;
			}

			return new Page { PhysicalOffset = offset, Size = size };
		}
	}
}