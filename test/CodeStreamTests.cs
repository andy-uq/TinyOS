using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Andy.TinyOS;
using NUnit.Framework;
using tinyOS;

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
				{ OpCode.Movi, null, 0, 1 }
			};

			Assert.That(stream.ToArray(), Is.Not.Empty);
		}

		[Test]
		public void Movi()
		{
			var stream = new CodeStream();

			stream.AsFluent()
				.Movi(Register.A, 0);

			Assert.That(stream.First(), Is.EqualTo(new Instruction { OpCode = OpCode.Movi, Parameters = new[] { 0U, 0U } }).Using(new InstructionComparer()));
		}

		[Test]
		public void Movr()
		{
			var stream = new CodeStream();

			stream.AsFluent()
				.Movr(Register.A, Register.B);

			Assert.That(stream.First(), Is.EqualTo(new Instruction { OpCode = OpCode.Movr, Parameters = new[] { 0U, 1U } }).Using(new InstructionComparer()));
		}
	}

	public class InstructionComparer : IEqualityComparer<Instruction>
	{
		public bool Equals(Instruction x, Instruction y)
		{
			if ( x.OpCode == y.OpCode && x.Parameters.Length == y.Parameters.Length )
			{
				return !x.Parameters.Where((t, i) => t != y.Parameters[i]).Any();
			}

			return false;
		}

		public int GetHashCode(Instruction obj)
		{
			throw new System.NotImplementedException();
		}
	}
}