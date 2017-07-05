using Andy.TinyOS;
using Xunit;

namespace ClassLibrary1
{
	public class OpCodeTests
	{
		[Fact]
		public void EncodeOpCode()
		{
			var op = new MaskedOpCode(OpCode.Add);
			Assert.That(op.OpCode, Is.EqualTo(OpCode.Add));
			Assert.That(op.Signed, Is.False);
		}

		[Fact]
		public void EncodeOpCodeAsString()
		{
			var op = new MaskedOpCode(OpCode.Add);
			Assert.That(op.ToString(), Is.EqualTo("Add"));
		}

		[Fact]
		public void EncodeOpCodeWithDestination()
		{
			var op = new MaskedOpCode(OpCode.Add).SetDest(OpCodeFlag.MemoryAddress);
			Assert.That(op.OpCode, Is.EqualTo(OpCode.Add));
			Assert.That(op.Signed, Is.False);

			Assert.That(op.Destination, Is.EqualTo(OpCodeFlag.MemoryAddress));
		}

		[Fact]
		public void EncodeOpCodeWithDestinationAsString()
		{
			var op = new MaskedOpCode(OpCode.Add)
				.SetDest(OpCodeFlag.MemoryAddress)
				.SetSource(OpCodeFlag.Constant);
			Assert.That(op.ToString(), Is.EqualTo("Add [rX] 0x0"));

			op = op.SetDest(OpCodeFlag.Register);
			Assert.That(op.ToString(), Is.EqualTo("Add rX 0x0"));

			op = op.SetDest(OpCodeFlag.MemoryAddress);
			Assert.That(op.ToString(), Is.EqualTo("Add [rX] 0x0"));
		}

		[Fact]
		public void EncodeOpCodeWithSource()
		{
			var op = new MaskedOpCode(OpCode.Add).SetSource(OpCodeFlag.Constant);
			Assert.That(op.OpCode, Is.EqualTo(OpCode.Add));
			Assert.That(op.Signed, Is.False);
			Assert.That(op.Source, Is.EqualTo(OpCodeFlag.Constant));
			Assert.That(op.Destination, Is.EqualTo(OpCodeFlag.None));
		}
		[Fact]
		public void EncodeOpCodeWithSourceAsString()
		{
			var op = new MaskedOpCode(OpCode.Add).SetDest(OpCodeFlag.MemoryAddress).SetSource(OpCodeFlag.Constant);
			Assert.That(op.ToString(), Is.EqualTo("Add [rX] 0x0"));

			op = op.SetSource(OpCodeFlag.Register);
			Assert.That(op.ToString(), Is.EqualTo("Add [rX] rX"));

			op = op.SetSource(OpCodeFlag.MemoryAddress);
			Assert.That(op.ToString(), Is.EqualTo("Add [rX] [rX]"));
		}

		[Fact]
		public void EncodeOpCodeAsSigned()
		{
			var op = new MaskedOpCode(OpCode.Add).AsSigned();
			Assert.That(op.OpCode, Is.EqualTo(OpCode.Add));
			Assert.That(op.Signed, Is.True);
		}
	}
}