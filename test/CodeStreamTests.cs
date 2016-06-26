using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Andy.TinyOS;
using NUnit.Framework;

namespace ClassLibrary1
{
	[TestFixture]
	public class CodeStreamTests
	{
		[Test]
		public void InitialiseCodeStream()
		{
			new CodeStream();
		}

		[Test]
		public void AsEnumerable()
		{
			var stream = new CodeStream();
			Assert.That(stream, Is.InstanceOf<IEnumerable<Instruction>>());
		}

		[Test]
		public void Enumerate()
		{
			var stream = new CodeStream();
			Assert.That(stream.AsEnumerable().ToArray(), Is.InstanceOf<Instruction[]>());
		}

		[Test]
		public void CanComposeSingleInstruction()
		{
			var stream = new CodeStream();
			var i1 = new Instruction();
			var s2 = stream + i1;

			Assert.That(s2, Is.EquivalentTo(new[] { i1 }));
		}

		[Test]
		public void CanComposeMultipleInstructions()
		{
			var i1 = new Instruction();
			var i2 = new Instruction();
			var s1 = new CodeStream { i1 };
			var s2 = new CodeStream {i2};
			var s3 = s1 + s2;

			Assert.That(s3, Is.EquivalentTo(new[] { i1, i2 }));
		}

		[Test]
		public void WriteInstruction()
		{
			var stream =new CodeStream();
			var instruction = new Instruction();
			stream.Add(instruction);

			Assert.That(stream.ToArray(), Is.EqualTo(new[] { instruction }));
		}

		[Test]
		public void WriteMultipleInstruction()
		{
			var stream =new CodeStream();
			var i1 = new Instruction();
			var i2 = new Instruction();
			stream.Add(i1, i2);

			Assert.That(stream.ToArray(), Is.EqualTo(new[] { i1, i2 }));
		}

		[Test]
		public void WriteExplicitInstruction()
		{
			var stream =new CodeStream
			{
				new Instruction(OpCode.Mov)
			};

			Assert.That(stream.ToArray(), Is.Not.Empty);
		}

		[Test]
		public void Movi()
		{
			var stream = new CodeStream();

			stream.AsFluent()
				.Mov.RI(Register.A, 0);

			Assert.That(stream.First(), Is.EqualTo(new Instruction(OpCode.Mov).Destination(Register.A).Source(0)).Using(new InstructionComparer()));
		}

		[Test]
		public void Movr()
		{
			var stream = new CodeStream();

			stream.AsFluent()
				.Mov.RR(Register.A, Register.B);

			var expected = new Instruction(OpCode.Mov).Source(Register.B).Destination(Register.A);
			Assert.That(stream.First(), Is.EqualTo(expected).Using(new InstructionComparer()));
		}
	}
}