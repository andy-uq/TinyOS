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

		public void Allocate(Page page)
		{
			_pages.Add(page);
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