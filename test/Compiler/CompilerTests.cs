using System;
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
		[TestCase("1", 1U)]
		[TestCase("!1", uint.MaxValue - 1)]
		[TestCase("!!1", 1U)]
		[TestCase("!(!1)", 1U)]
		[TestCase("-1", uint.MaxValue)]
		[TestCase("3&2", 2U)]
		[TestCase("5&3&1", 1U)]
		[TestCase("15&12&4", 4U)]
		[TestCase("(15&12)&4", 4U)]
		[TestCase("1|2", 3U)]
		[TestCase("4^7", 3U)]
		[TestCase("4+7", 11U)]
		[TestCase("7-3", 4U)]
		[TestCase("10-10", 0U)]
		[TestCase("10-1+5", 14U)]
		[TestCase("(10-1)+5", 14U)]
		[TestCase("10-(1+5)", 4U)]
		[TestCase("2*(1+2)", 6U)]
		[TestCase("2*1+2", 4U)]
		public void SingleExpression(string source, uint result)
		{
			var grammar = new AndyStructuralGrammar();
			var parserState = new ParserState(source);

			var parseResult = grammar.expression.Match(parserState);
			Assert.That(parseResult, Is.True);

			var compiler = new Andy.TinyOS.Compiler.Compiler(grammar, parserState);
			var program = compiler.Compile();

			var r = Run(program);
			Assert.That(r, Is.EqualTo(result));
		}

		private uint Run(IEnumerable<Instruction> program)
		{
			var cpu = new Cpu();
			IdleProcess.Initialise(cpu);

			var prog1 = cpu.Load();

			var stream = cpu.Compile(prog1, program);
			Console.WriteLine("Code Size: {0} bytes", stream.Length);

			cpu.Run(prog1);

			while (prog1.IsRunning)
				cpu.Tick();

			return prog1.ExitCode;
		}
	}
}