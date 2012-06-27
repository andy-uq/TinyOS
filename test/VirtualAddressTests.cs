using NUnit.Framework;
using tinyOS;

namespace ClassLibrary1
{
	[TestFixture]
	public class VirtualAddressTests
	{
		[Test]
		public void Zero()
		{
			var v1 = new VirtualAddress(0);
			Assert.That(v1.PageNumber, Is.EqualTo(0));
			Assert.That(v1.Offset, Is.EqualTo(0));
		}

		[Test]
		public void MaxValue()
		{
			var v1 = new VirtualAddress(uint.MaxValue);
			Assert.That(v1.Offset, Is.EqualTo((1 << 12) - 1));
			Assert.That(v1.PageNumber, Is.EqualTo((1 << 20) - 1));
		}

		[Test]
		public void Page()
		{
			var v1 = new VirtualAddress(1 << 12);
			Assert.That(v1.Offset, Is.EqualTo(0));
			Assert.That(v1.PageNumber, Is.EqualTo(1));
		}

		[Test]
		public void PageToAddr()
		{
			var v1 = new VirtualAddress(0, 1);
			Assert.That(v1.Address, Is.EqualTo(1 << 12));
		}

		[Test]
		public void Offset()
		{
			var v1 = new VirtualAddress(1024);
			Assert.That(v1.Offset, Is.EqualTo(1024));
			Assert.That(v1.PageNumber, Is.EqualTo(0));
		}

		[Test]
		public void OffsetToAddr()
		{
			var v1 = new VirtualAddress(1024, 0);
			Assert.That(v1.Address, Is.EqualTo(1024));
		}
	}
}