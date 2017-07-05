using Andy.TinyOS;
using FluentAssertions;
using Xunit;

namespace ClassLibrary1
{
	public class OpCodeTests
	{
		[Fact]
		public void EncodeOpCode()
		{
			var op = new MaskedOpCode(OpCode.Add);
			op.OpCode.Should().Be(OpCode.Add);
			op.Signed.Should().BeFalse();
		}

		[Fact]
		public void EncodeOpCodeAsString()
		{
			var op = new MaskedOpCode(OpCode.Add);
			op.ToString().Should().Be("Add");
		}

		[Fact]
		public void EncodeOpCodeWithDestination()
		{
			var op = new MaskedOpCode(OpCode.Add).SetDest(OpCodeFlag.MemoryAddress);
			op.OpCode.Should().Be(OpCode.Add);
			op.Signed.Should().BeFalse();

			op.Destination.Should().Be(OpCodeFlag.MemoryAddress);
		}

		[Fact]
		public void EncodeOpCodeWithDestinationAsString()
		{
			var op = new MaskedOpCode(OpCode.Add)
				.SetDest(OpCodeFlag.MemoryAddress)
				.SetSource(OpCodeFlag.Constant);
			op.ToString().Should().Be("Add [rX] 0x0");

			op = op.SetDest(OpCodeFlag.Register);
			op.ToString().Should().Be("Add rX 0x0");

			op = op.SetDest(OpCodeFlag.MemoryAddress);
			op.ToString().Should().Be("Add [rX] 0x0");
		}

		[Fact]
		public void EncodeOpCodeWithSource()
		{
			var op = new MaskedOpCode(OpCode.Add).SetSource(OpCodeFlag.Constant);
			op.OpCode.Should().Be(OpCode.Add);
			op.Signed.Should().BeFalse();
			op.Source.Should().Be(OpCodeFlag.Constant);
			op.Destination.Should().Be(OpCodeFlag.None);
		}
		[Fact]
		public void EncodeOpCodeWithSourceAsString()
		{
			var op = new MaskedOpCode(OpCode.Add).SetDest(OpCodeFlag.MemoryAddress).SetSource(OpCodeFlag.Constant);
			op.ToString().Should().Be("Add [rX] 0x0");

			op = op.SetSource(OpCodeFlag.Register);
			op.ToString().Should().Be("Add [rX] rX");

			op = op.SetSource(OpCodeFlag.MemoryAddress);
			op.ToString().Should().Be("Add [rX] [rX]");
		}

		[Fact]
		public void EncodeOpCodeAsSigned()
		{
			var op = new MaskedOpCode(OpCode.Add).AsSigned();
			op.OpCode.Should().Be(OpCode.Add);
			op.Signed.Should().BeTrue();
		}
	}
}