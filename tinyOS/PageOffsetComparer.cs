using System.Collections.Generic;

namespace tinyOS
{
	public class PageOffsetComparer : IComparer<Page>
	{
		public int Compare(Page x, Page y)
		{
			var result = x.PhysicalOffset.CompareTo(y.PhysicalOffset);
			return Forward ? result : result*-1;
		}

		public bool Forward { get; set; }
	}
}