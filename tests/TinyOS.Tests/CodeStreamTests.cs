using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Andy.TinyOS;
using FluentAssertions;
using Xunit;

namespace ClassLibrary1
{
	public class CodeStreamTests
	{
		[Fact]
		public void InitialiseCodeStream()
		{
			new CodeStream();
		}

		[Fact]
		public void AsEnumerable()
		{
			var stream = new CodeStream();
			global::ClassLibrary1.Assert.That(stream, Is.InstanceOf<IEnumerable<Instruction>>());
		}

		[Fact]
		public void Enumerate()
		{
			var stream = new CodeStream();
			Assert.That(stream.AsEnumerable().ToArray(), Is.InstanceOf<Instruction[]>());
		}

		[Fact]
		public void CanComposeSingleInstruction()
		{
			var stream = new CodeStream();
			var i1 = new Instruction();
			var s2 = stream + i1;

			Assert.That(s2, Is.EquivalentTo(new[] { i1 }));
		}

		[Fact]
		public void CanComposeMultipleInstructions()
		{
			var i1 = new Instruction();
			var i2 = new Instruction();
			var s1 = new CodeStream { i1 };
			var s2 = new CodeStream {i2};
			var s3 = s1 + s2;

			Assert.That(s3, Is.EquivalentTo(new[] { i1, i2 }));
		}

		[Fact]
		public void WriteInstruction()
		{
			var stream =new CodeStream();
			var instruction = new Instruction();
			stream.Add(instruction);

			stream.ToArray().Should().Be(new[] { instruction });
		}

		[Fact]
		public void WriteMultipleInstruction()
		{
			var stream =new CodeStream();
			var i1 = new Instruction();
			var i2 = new Instruction();
			stream.Add(i1, i2);

			stream.ToArray().Should().Be(new[] { i1, i2 });
		}

		[Fact]
		public void WriteExplicitInstruction()
		{
			var stream =new CodeStream
			{
				new Instruction(OpCode.Mov)
			};

			Assert.That(stream.ToArray(), Is.Not.Empty);
		}

		[Fact]
		public void Movi()
		{
			var stream = new CodeStream();

			stream.AsFluent()
				.Mov.RI(Register.A, 0);

			var expected = new Instruction(OpCode.Mov).Destination(Register.A).Source(0);
			stream.First().Should().Be(expected, new InstructionComparer());
		}

		[Fact]
		public void Movr()
		{
			var stream = new CodeStream();

			stream.AsFluent()
				.Mov.RR(Register.A, Register.B);

			var expected = new Instruction(OpCode.Mov).Source(Register.B).Destination(Register.A);
			stream.First().Should().Be(expected, new InstructionComparer());
		}
	}
}