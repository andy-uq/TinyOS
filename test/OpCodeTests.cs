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
			var op = new MaskedOpCode(OpCode.Add);
			Assert.That(op.OpCode, Is.EqualTo(OpCode.Add));
			Assert.That(op.Signed, Is.False);
		}

		[Test]
		public void EncodeOpCodeAsString()
		{
			var op = new MaskedOpCode(OpCode.Add);
			Assert.That(op.ToString(), Is.EqualTo("Add"));
		}

		[Test]
		public void EncodeOpCodeWithDestination()
		{
			var op = new MaskedOpCode(OpCode.Add).SetDest(OpCodeFlag.MemoryAddress);
			Assert.That(op.OpCode, Is.EqualTo(OpCode.Add));
			Assert.That(op.Signed, Is.False);

			Assert.That(op.Destination, Is.EqualTo(OpCodeFlag.MemoryAddress));
		}

		[Test]
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

		[Test]
		public void EncodeOpCodeWithSource()
		{
			var op = new MaskedOpCode(OpCode.Add).SetSource(OpCodeFlag.Constant);
			Assert.That(op.OpCode, Is.EqualTo(OpCode.Add));
			Assert.That(op.Signed, Is.False);
			Assert.That(op.Source, Is.EqualTo(OpCodeFlag.Constant));
			Assert.That(op.Destination, Is.EqualTo(OpCodeFlag.None));
		}
		[Test]
		public void EncodeOpCodeWithSourceAsString()
		{
			var op = new MaskedOpCode(OpCode.Add).SetDest(OpCodeFlag.MemoryAddress).SetSource(OpCodeFlag.Constant);
			Assert.That(op.ToString(), Is.EqualTo("Add [rX] 0x0"));

			op = op.SetSource(OpCodeFlag.Register);
			Assert.That(op.ToString(), Is.EqualTo("Add [rX] rX"));

			op = op.SetSource(OpCodeFlag.MemoryAddress);
			Assert.That(op.ToString(), Is.EqualTo("Add [rX] [rX]"));
		}

		[Test]
		public void EncodeOpCodeAsSigned()
		{
			var op = new MaskedOpCode(OpCode.Add).AsSigned();
			Assert.That(op.OpCode, Is.EqualTo(OpCode.Add));
			Assert.That(op.Signed, Is.True);
		}
	}
}