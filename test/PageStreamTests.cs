using Andy.TinyOS;
using NUnit.Framework;
using tinyOS;

namespace ClassLibrary1
{
	[TestFixture]
	public class PageStreamTests
	{
		[Test]
		public void Read()
		{
			var buffer = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
			const uint frameSize = 4;
			var ram = new Ram(buffer, frameSize);
			var pageSet = new PageInfo();

			var pcb = new ProcessContextBlock {Id = 1};
			ram.Allocate(pcb);
			pageSet.Append(ram.Allocate(pcb));
			pageSet.Append(ram.Allocate(pcb));

			var pageStream = new PageStream(ram, pageSet);
			Assert.That(pageStream.Length, Is.EqualTo(8));

			var b = new byte[6];
			pageStream.Read(b, 0, 6);

			Assert.That(pageStream.Position, Is.EqualTo(6));

			Assert.That(b, Is.EquivalentTo(new[] { 5, 6, 7, 8, 9, 10 }));
		}

		[Test]
		public void Write()
		{
			var buffer = new byte[] {1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
			const uint frameSize = 4;
			var ram = new Ram(buffer, frameSize);
			var pageSet = new PageInfo();

			var pcb = new ProcessContextBlock {Id = 1};
			ram.Allocate(pcb);
			pageSet.Append(ram.Allocate(pcb));
			pageSet.Append(ram.Allocate(pcb));

			var pageStream = new PageStream(ram, pageSet);
			Assert.That(pageStream.Length, Is.EqualTo(8));

			var b = new byte[] { 91, 92, 93, 94, };
			pageStream.Position = 2;
			pageStream.Write(b, 0, 4);

			Assert.That(buffer, Is.EquivalentTo(new[] { 1, 2, 3, 4, 5, 6, 91, 92, 93, 94, 11, 12, }));
		}
	}
}