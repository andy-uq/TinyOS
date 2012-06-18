using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace tinyOS
{
	public class PageSizeComparer : IComparer<Page>
	{
		public int Compare(Page x, Page y)
		{
			return x.Size.CompareTo(y.Size);
		}
	}
	public class PageOffsetComparer : IComparer<Page>
	{
		public int Compare(Page x, Page y)
		{
			var result = x.Offset.CompareTo(y.Offset);
			return Forward ? result : result*-1;
		}

		public bool Forward { get; set; }
	}

	public class MemoryManager
	{
		private readonly SortedSet<Page> _pages;
		private readonly SortedSet<Page> _freePages;

		public MemoryManager(uint freeSize)
		{
			_pages = new SortedSet<Page>(new PageOffsetComparer());
			_freePages = new SortedSet<Page>(new PageSizeComparer()) {new Page {Offset = 0, Size = freeSize}};
		}

		public IEnumerable<PageInfo> AllocatedPages
		{
			get { return _pages.Reverse().Select(page => new PageInfo(page)); }
		}

		public IEnumerable<PageInfo> FreePages
		{
			get { return _freePages.Select(page => new PageInfo(page)); }
		}

		public Page Allocate(int owner, uint size)
		{
			var freeBlock = _freePages.FirstOrDefault(x => size <= x.Size);
			if ( freeBlock == null )
				return null;

			var page = new Page { Owner = owner, Offset = freeBlock.Offset, Size = size};

			freeBlock.Offset += size;
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
					return (int)(page.Offset + vAddr);

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

				freePage.Offset = GetPagesToRight(freePage)
					.Reverse()
					.Aggregate(freePage.Offset, (offset, pageToTheRight) => MovePage(pageToTheRight, offset));

				CollapseFreePage(freePage);
			}
		}

		private uint MovePage(Page page, uint newOffset)
		{
			var offset = page.Offset;
			page.Offset = newOffset;

			return offset;
		}

		private IEnumerable<Page> GetPagesToRight(Page page)
		{
			return _pages.TakeWhile(x => x.Offset > page.Offset);
		}

		private Page CollapseFreePage(Page page)
		{
			uint offset = page.Offset;
			uint size = page.Size;

			Page left;
			while ( (left = _freePages.SingleOrDefault(x => (x.Offset + x.Size) == offset)) != null )
			{
				_freePages.Remove(left);
				offset = left.Offset;
				size += left.Size;
			}

			Page right;
			while ( (right = _freePages.SingleOrDefault(x => (offset + size) == x.Offset)) != null )
			{
				_freePages.Remove(right);
				size += right.Size;
			}

			return new Page { Offset = offset, Size = size };
		}

		public int Compare(Page x, Page y)
		{
			return x.Size.CompareTo(y.Size);
		}
	}
}