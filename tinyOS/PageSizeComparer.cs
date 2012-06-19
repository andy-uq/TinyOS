using System.Collections.Generic;

namespace tinyOS
{
	public class PageSizeComparer : IComparer<Page>
	{
		public int Compare(Page x, Page y)
		{
			return x.Size.CompareTo(y.Size);
		}
	}
}