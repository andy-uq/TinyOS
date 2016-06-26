using System;
using System.Collections.Generic;
using System.Linq;

namespace Andy.TinyOS
{
	public class PageInfo
	{
		private Page _page;
		private readonly List<Page> _pages;

		public PageInfo()
		{
			_pages = new List<Page>();
		}

		public uint Offset => _page?.VirtualAddress.Address ?? 0;

		public uint Size
		{
			get { return (uint) _pages.Sum(x => x.Size); }
		}

		public IEnumerable<Page> Pages => _pages;

		public override string ToString()
		{
			return $"[0x{Offset:x4}] ({Size:n0} bytes)";
		}

		public void Append(Page page)
		{
			if (page == null)
				throw new ArgumentNullException(nameof(page));

			if (_page == null)
				_page = page;

			_pages.Add(page);
		}

		public Page Next(Page page)
		{
			var pageIndex = _pages.IndexOf(page) + 1;
			return pageIndex == _pages.Count ? null : _pages[pageIndex];
		}

		public Page Find(uint pageNumber)
		{
			var page = Pages.SingleOrDefault(x => x.PageNumber == pageNumber);
			if (page != null)
				return page;

			throw new InvalidOperationException("Cannot find page " + pageNumber);
		}
	}
}