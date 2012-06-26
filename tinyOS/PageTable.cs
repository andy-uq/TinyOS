using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace tinyOS
{
	public class PageTable : IEnumerable<Page>
	{
		private readonly List<PageInfo> _pages;
	    private uint _nextPageNo;

		public PageTable()
		{
			_pages = new List<PageInfo>();
		    _nextPageNo = 1;
		}

		public Page CreatePage()
		{
		    uint pageNumber = _nextPageNo;
		    var page = new Page {PageNumber = pageNumber };

		    _nextPageNo++;
		    return page;
		}

        public PageInfo Find(Predicate<PageInfo> match)
        {
            return _pages.Find(match);
        }

	    public PageInfo Find(uint vAddr)
	    {
	        return Find(x => x.Offset <= vAddr && vAddr >= x.Offset + x.Size);
	    }

        public void Add(PageInfo page)
        {
            _pages.Add(page);
        }

		public void Free(PageInfo page)
		{
			_pages.Remove(page);
		}

		public IEnumerator<Page> GetEnumerator()
		{
			return _pages.SelectMany(x => x.Pages).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}