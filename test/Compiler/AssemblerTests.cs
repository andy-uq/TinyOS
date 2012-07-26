using System;
using System.IO;
using System.Linq;
using Andy.TinyOS;
using Andy.TinyOS.Compiler;
using Andy.TinyOS.Parser;
using Andy.TinyOS.Utility;
using NUnit.Framework;

namespace ClassLibrary1.Compiler
{
	[TestFixture]
	public class AssemblerTests
	{
		[Test]
		public void AssembleSimple()
		{
			var x = new[]
			{
				new Instruction(OpCode.Mov, "Put 1 into r1").Destination(Register.A).Source(1),
				new Instruction(OpCode.Exit, "Exit with r1").Source(Register.A),
			};

			var grammer = new AsmStructuralGrammar();
			var p = new ParserState(TestPrograms.ReallySimpleProgram);

			bool parseResult = grammer.program.Match(p);
			Assert.That(parseResult, Is.True);

			var assembler = new Assembler(p);
			var code = assembler.Assemble().ToList();

			using (var w = new InstructionTextWriter(Console.Out))
				code.ForEach(w.Write);

			Assert.That(code, Is.EquivalentTo(x).Using(new InstructionComparer()));
		}
	}
}