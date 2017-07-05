using System;
using Andy.TinyOS;
using FluentAssertions;
using Xunit;

namespace ClassLibrary1
{
	public class VirtualAddressTests
	{
		private VirtualAddressCalculator _calculator;

		public VirtualAddressTests()
		{
			_calculator = new VirtualAddressCalculator(4096);
			_calculator.PageSize.Should().Be(4096);
			_calculator.MaxPages.Should().Be(1 << 20);
			_calculator.PageShift.Should().Be(12);
			_calculator.OffsetMask.Should().Be(4095);
		}

		[Fact]
		public void BadPageArgumentThrows()
		{
			Xunit.Assert.Throws<ArgumentOutOfRangeException>(() => _calculator.New(-1, 0));
		}

		[Fact]
		public void BadOffsetArgumentThrows()
		{
			Xunit.Assert.Throws<ArgumentOutOfRangeException>(() => _calculator.New(0, -1));
		}

		[Fact]
		public void Zero()
		{
			var v1 = _calculator.New(0);
			v1.PageNumber.Should().Be(0);
			v1.Offset.Should().Be(0);
		}

		[Fact]
		public void MaxValue()
		{
			var v1 = _calculator.New(uint.MaxValue);
			Assert.That(v1.Offset, Is.EqualTo((1 << 12) - 1));
			Assert.That(v1.PageNumber, Is.EqualTo((1 << 20) - 1));
		}

		[Fact]
		public void Page()
		{
			var v1 = _calculator.New(1 << 12);
			v1.Offset.Should().Be(0);
			v1.PageNumber.Should().Be(1);
		}

		[Fact]
		public void PageToAddr()
		{
			var v1 = _calculator.New(0, 1);
			v1.Address.Should().Be(1 << 12);

			var v2 = _calculator.New(0, 2);
			v2.Address.Should().Be(2 << 12);
		}

		[Fact]
		public void AddAddr()
		{
			var v1 = _calculator.New(4095, 0);
			var v2 = v1 + 1;

			v2.Address.Should().Be(4096);
			v2.PageNumber.Should().Be(1);
			v2.Offset.Should().Be(0);
		}

		[Fact]
		public void SubAddr()
		{
			var v1 = _calculator.New(0, 2);
			var v2 = v1 - 1;

			v2.Address.Should().Be(8191);
			v2.PageNumber.Should().Be(1);
			v2.Offset.Should().Be(4095);
		}

		[Fact]
		public void Offset()
		{
			var v1 = _calculator.New(1024);
			v1.Offset.Should().Be(1024);
			v1.PageNumber.Should().Be(0);
		}

		[Fact]
		public void OffsetToAddr()
		{
			var v1 = _calculator.New(1024, 0);
			v1.Address.Should().Be(1024);
		}

		[Fact]
		public void CanToString()
		{
			var v1 = _calculator.New(1024, 0);
			v1.ToString().Should().Be("[0x00000400] Page: 0, Offset: 1024");
		}
	}
}