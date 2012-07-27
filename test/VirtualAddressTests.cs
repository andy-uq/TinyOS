using System;
using Andy.TinyOS;
using NUnit.Framework;
using tinyOS;

namespace ClassLibrary1
{
	[TestFixture]
	public class VirtualAddressTests
	{
		private VirtualAddressCalculator _calculator;

		[SetUp]
		public void SetUp()
		{
			_calculator = new VirtualAddressCalculator(4096);
			Assert.That(_calculator.PageSize, Is.EqualTo(4096));
			Assert.That(_calculator.MaxPages, Is.EqualTo(1 << 20));
			Assert.That(_calculator.PageShift, Is.EqualTo(12));
			Assert.That(_calculator.OffsetMask, Is.EqualTo(4095));
		}

		[Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void BadPageArgumentThrows()
		{
			_calculator.New(-1, 0);
		}

		[Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void BadOffsetArgumentThrows()
		{
			_calculator.New(0, -1);
		}

		[Test]
		public void Zero()
		{
			var v1 = _calculator.New(0);
			Assert.That(v1.PageNumber, Is.EqualTo(0));
			Assert.That(v1.Offset, Is.EqualTo(0));
		}

		[Test]
		public void MaxValue()
		{
			var v1 = _calculator.New(uint.MaxValue);
			Assert.That(v1.Offset, Is.EqualTo((1 << 12) - 1));
			Assert.That(v1.PageNumber, Is.EqualTo((1 << 20) - 1));
		}

		[Test]
		public void Page()
		{
			var v1 = _calculator.New(1 << 12);
			Assert.That(v1.Offset, Is.EqualTo(0));
			Assert.That(v1.PageNumber, Is.EqualTo(1));
		}

		[Test]
		public void PageToAddr()
		{
			var v1 = _calculator.New(0, 1);
			Assert.That(v1.Address, Is.EqualTo(1 << 12));

			var v2 = _calculator.New(0, 2);
			Assert.That(v2.Address, Is.EqualTo(2 << 12));
		}

		[Test]
		public void AddAddr()
		{
			var v1 = _calculator.New(4095, 0);
			var v2 = v1 + 1;

			Assert.That(v2.Address, Is.EqualTo(4096));
			Assert.That(v2.PageNumber, Is.EqualTo(1));
			Assert.That(v2.Offset, Is.EqualTo(0));
		}

		[Test]
		public void SubAddr()
		{
			var v1 = _calculator.New(0, 2);
			var v2 = v1 - 1;

			Assert.That(v2.Address, Is.EqualTo(8191));
			Assert.That(v2.PageNumber, Is.EqualTo(1));
			Assert.That(v2.Offset, Is.EqualTo(4095));
		}

		[Test]
		public void Offset()
		{
			var v1 = _calculator.New(1024);
			Assert.That(v1.Offset, Is.EqualTo(1024));
			Assert.That(v1.PageNumber, Is.EqualTo(0));
		}

		[Test]
		public void OffsetToAddr()
		{
			var v1 = _calculator.New(1024, 0);
			Assert.That(v1.Address, Is.EqualTo(1024));
		}

		[Test]
		public void CanToString()
		{
			var v1 = _calculator.New(1024, 0);
			Assert.That(v1.ToString(), Is.EqualTo("[0x00000400] Page: 0, Offset: 1024"));
		}
	}
}