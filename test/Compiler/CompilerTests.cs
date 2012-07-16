using System.Collections.Generic;
using System.Linq;
using Andy.TinyOS;
using Andy.TinyOS.Parser;
using NUnit.Framework;
using tinyOS;

namespace ClassLibrary1.Compiler
{
	public class CompilerTests
	{
		[TestCase("1", 1)]
		[TestCase("99", 99)]
		[TestCase("10+20", 30)]
		public void SingleExpression(string source, int result)
		{
			var grammar = new AndyStructuralGrammer();
			var parserState = new ParserState(source);

			var parseResult = grammar.expression.Match(parserState);
			Assert.That(parseResult, Is.True);

			var compiler = new Andy.TinyOS.Compiler.Compiler<AndyStructuralGrammer>(grammar, parserState);
			var program = compiler.Compile();

			var r = Run(program);
			Assert.That(r, Is.EqualTo(parseResult));
		}

		private uint Run(IEnumerable<Instruction> program)
		{
			Cpu cpu = new Cpu();
			var prog1 = cpu.Load();

			cpu.Compile(prog1, program);
			cpu.Run(prog1);

			while (prog1.IsRunning)
				cpu.Tick();

			return prog1.ExitCode;
		}
	}
}