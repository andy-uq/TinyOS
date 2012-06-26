using System;
using System.Collections.Generic;
using System.Linq;

namespace tinyOS
{
	public class PageInfo
	{
	    private Page _page;
	    private readonly List<Page> _pages;

	    public PageInfo()
	    {
            _pages = new List<Page>();
	    }

	    public uint Owner { get; private set; }
        public uint Offset { get { return _page == null ? 0 : _page.VirtualAddress.Address; } }
        public uint Size { get { return (uint) _pages.Sum(x => x.Size); } }

	    public IEnumerable<Page> Pages
	    {
	        get { return _pages; }
	    }

	    public override string ToString()
		{
			return string.Format("[0x{0:x4}] ({1:n0} bytes)", Offset, Size);
		}

	    public void Append(Page page)
	    {
            if (page == null)
                throw new ArgumentNullException("page");

            if (_page == null)
                _page = page;

	        _pages.Add(page);
	    }

	    public Page Next(Page page)
	    {
	        var pageIndex = _pages.IndexOf(page) + 1;
            return pageIndex == _pages.Count ? null : _pages[pageIndex];
	    }
	}
}