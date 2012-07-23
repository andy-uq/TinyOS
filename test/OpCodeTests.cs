using Andy.TinyOS;
using NUnit.Framework;

namespace ClassLibrary1
{
	[TestFixture]
	public class OpCodeTests
	{
		[Test]
		public void EncodeOpCode()
		{
			var op = new MaskedOpCode(OpCode.Addi);
			Assert.That(op.OpCode, Is.EqualTo(OpCode.Addi));
			Assert.That(op.Signed, Is.False);
		}

		[Test]
		public void EncodeOpCodeAsString()
		{
			var op = new MaskedOpCode(OpCode.Addi);
			Assert.That(op.ToString(), Is.EqualTo("Addi"));
		}

		[Test]
		public void EncodeOpCodeWithDestination()
		{
			var op = new MaskedOpCode(OpCode.Addi).SetDest(OpCodeFlag.MemoryAddress);
			Assert.That(op.OpCode, Is.EqualTo(OpCode.Addi));
			Assert.That(op.Signed, Is.False);

			Assert.That(op.Destination, Is.EqualTo(OpCodeFlag.MemoryAddress));
		}

		[Test]
		public void EncodeOpCodeWithDestinationAsString()
		{
			var op = new MaskedOpCode(OpCode.Addi)
				.SetDest(OpCodeFlag.MemoryAddress)
				.SetSource(OpCodeFlag.Constant);
			Assert.That(op.ToString(), Is.EqualTo("Addi [rX] 0x0"));

			op = op.SetDest(OpCodeFlag.Register);
			Assert.That(op.ToString(), Is.EqualTo("Addi rX 0x0"));

			op = op.SetDest(OpCodeFlag.MemoryAddress);
			Assert.That(op.ToString(), Is.EqualTo("Addi [rX] 0x0"));
		}

		[Test]
		public void EncodeOpCodeWithSource()
		{
			var op = new MaskedOpCode(OpCode.Addi).SetSource(OpCodeFlag.Constant);
			Assert.That(op.OpCode, Is.EqualTo(OpCode.Addi));
			Assert.That(op.Signed, Is.False);
			Assert.That(op.Source, Is.EqualTo(OpCodeFlag.Constant));
			Assert.That(op.Destination, Is.EqualTo(OpCodeFlag.None));
		}
		[Test]
		public void EncodeOpCodeWithSourceAsString()
		{
			var op = new MaskedOpCode(OpCode.Addi).SetDest(OpCodeFlag.MemoryAddress).SetSource(OpCodeFlag.Constant);
			Assert.That(op.ToString(), Is.EqualTo("Addi [rX] 0x0"));

			op = op.SetSource(OpCodeFlag.Register);
			Assert.That(op.ToString(), Is.EqualTo("Addi [rX] rX"));

			op = op.SetSource(OpCodeFlag.MemoryAddress);
			Assert.That(op.ToString(), Is.EqualTo("Addi [rX] [rX]"));
		}

		[Test]
		public void EncodeOpCodeAsSigned()
		{
			var op = new MaskedOpCode(OpCode.Addi).AsSigned();
			Assert.That(op.OpCode, Is.EqualTo(OpCode.Addi));
			Assert.That(op.Signed, Is.True);
		}
	}
}