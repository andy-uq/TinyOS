using System.Linq;
using NUnit.Framework;
using tinyOS;

namespace ClassLibrary1
{
	[TestFixture]
	public class MemoryManagerTests
	{
        ProcessContextBlock _pcb = new ProcessContextBlock();

		public MemoryManagerTests()
		{
		}

		[Test]
		public void Allocate()
		{
			var mm = new MemoryManager(new Ram(10, 1));

			var p1 = mm.Allocate(_pcb);
			var p2 = mm.Allocate(_pcb);
			var p3 = mm.Allocate(_pcb);

			var allocated = mm.AllocatedPages.ToArray();
			Assert.That(allocated.Length, Is.EqualTo(3));
			Assert.That(new PageInfo(p1), Is.EqualTo(allocated[0]));
			Assert.That(new PageInfo(p2), Is.EqualTo(allocated[1]));
			Assert.That(new PageInfo(p3), Is.EqualTo(allocated[2]));
		}

		[Test]
		public void Free()
		{
            var mm = new MemoryManager(new Ram(10, 1));

            var p1 = mm.Allocate(_pcb);
            var p2 = mm.Allocate(_pcb);
            var p3 = mm.Allocate(_pcb);

			var freeBlock = mm.FreePages.Single();
			Assert.That(freeBlock.Offset, Is.EqualTo(6));
			Assert.That(freeBlock.Size, Is.EqualTo(4));
		}

		[Test]
		public void FreeLeft()
		{
            var mm = new MemoryManager(new Ram(10, 1));

            var p1 = mm.Allocate(_pcb);
            var p2 = mm.Allocate(_pcb);
            var p3 = mm.Allocate(_pcb);

			mm.Free(p1);

			var freePages = mm.FreePages.ToArray();
			Assert.That(freePages[0].Offset, Is.EqualTo(0));
			Assert.That(freePages[1].Offset, Is.EqualTo(6));

			mm.Free(p3);

			freePages = mm.FreePages.ToArray();
			Assert.That(freePages[0].Offset, Is.EqualTo(0));
			Assert.That(freePages[1].Offset, Is.EqualTo(3));

			mm.Free(p2);

			freePages = mm.FreePages.ToArray();
			Assert.That(freePages.Length, Is.EqualTo(1));
			Assert.That(freePages[0].Offset, Is.EqualTo(0));
			Assert.That(freePages[0].Size, Is.EqualTo(10));
		}
	}
}