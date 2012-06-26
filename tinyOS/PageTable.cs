using System;
using System.Collections;
using System.Collections.Generic;

namespace tinyOS
{
	public class PageTable : IEnumerable<Page>
	{
		private readonly SortedSet<Page> _pages;

		public PageTable()
		{
			_pages = new SortedSet<Page>(new PageOffsetComparer { Forward = true });
		}

		public Page Allocate()
		{
		    uint pageNumber = (uint) _pages.Count + 1;
		    var page = new Page {Number = pageNumber };

		    _pages.Add(page);
		    return page;
		}

		public void Free(Page page)
		{
			_pages.Remove(page);
		}

		public IEnumerator<Page> GetEnumerator()
		{
			return _pages.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}