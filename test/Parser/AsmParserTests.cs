using System;
using Andy.TinyOS.Parser;
using NUnit.Framework;

namespace ClassLibrary1.Parser
{
	[TestFixture]
	public class AsmParserTests
	{
		[TestCase("mov", true)]
		public void ParseOpCode(string value, bool expected)
		{
			var grammer = new AsmBaseGrammar();
			var printer = new CppStructuralOutputAsXml();
			var p = new ParserState(value);

			Assert.That(grammer.opcode.Match(p), Is.EqualTo(expected));

			ParseNode node = p.GetRoot();
			Console.WriteLine(node.Value);
		}

		[TestCase("r0", true)]
		[TestCase("r1", true)]
		[TestCase("r2", true)]
		[TestCase("r3", true)]
		[TestCase("r4", true)]
		[TestCase("r5", true)]
		[TestCase("r10", true)]
		[TestCase("rX", false)]
		public void ParseRegister(string value, bool expected)
		{
			var grammer = new AsmBaseGrammar();
			var p = new ParserState(value);

			Assert.That(grammer.register.Match(p), Is.EqualTo(expected));

			ParseNode node = p.GetRoot();
			Console.WriteLine(node.Value);
		}

		[TestCase("[r0]", true)]
		[TestCase("[r1]", true)]
		[TestCase("[r2]", true)]
		[TestCase("[r3]", true)]
		[TestCase("[r4]", true)]
		[TestCase("[r5]", true)]
		[TestCase("[r10]", true)]
		[TestCase("[rX]", false)]
		public void ParseMemoryAddress(string value, bool expected)
		{
			var grammer = new AsmBaseGrammar();
			var p = new ParserState(value);

			Assert.That(grammer.memoryAddress.Match(p), Is.EqualTo(expected));

			ParseNode node = p.GetRoot();
			Console.WriteLine(node.Value);
		}

		[TestCase("$1", true)]
		[TestCase("-1", true)]
		public void ParseConstant(string value, bool expected)
		{
			var grammer = new AsmBaseGrammar();
			var p = new ParserState(value);

			Assert.That(grammer.constant.Match(p), Is.EqualTo(expected));

			ParseNode node = p.GetRoot();
			Console.WriteLine(node.Value);
		}

		[TestCase("r0", true)]
		[TestCase("rX", false)]
		[TestCase("[r0]", true)]
		[TestCase("[rX]", false)]
		[TestCase("$1", true)]
		[TestCase("-1", true)]
		public void ParseOperand(string value, bool expected)
		{
			var grammer = new AsmBaseGrammar();
			var p = new ParserState(value);

			Assert.That(grammer.operand.Match(p), Is.EqualTo(expected));
			if ( !expected )
				return;

			var printer = new CppStructuralOutputAsXml();
			printer.Print(p.GetRoot());
			Console.WriteLine(printer.AsXml());
		}

		[TestCase("; Hi", true)]
		public void ParseComment(string value, bool expected)
		{
			var grammer = new AsmBaseGrammar();
			var p = new ParserState(value);

			Assert.That(grammer.comment.Match(p), Is.EqualTo(expected));

			var printer = new CppStructuralOutputAsXml();
			printer.Print(p.GetRoot());
			Console.WriteLine(printer.AsXml());
		}

		[TestCase("; Hi", true)]
		[TestCase("mov r1 $10 ;Move 10 into r1", true)]
		[TestCase("mov r1 $10", true)]
		[TestCase("ret", true)]
		[TestCase("exit r1", true)]
		[TestCase("mov   	r1  $3276834	;move a big number into r1", true)]
		[TestCase("pop		[r3]", true)]
		[TestCase("je r7		; je over the exit", true)]
		public void ParseLine(string value, bool expected)
		{
			var grammer = new AsmStructuralGrammar();
			var p = new ParserState(value);

			Assert.That(grammer.line.Match(p), Is.EqualTo(expected));

			var printer = new CppStructuralOutputAsXml();
			printer.Print(p.GetRoot());
			Console.WriteLine(printer.AsXml());
		}

		[TestCase("; Hi", true)]
		[TestCase("mov r1 $10 ;Move 10 into r1", true)]
		[TestCase(@"mov r1 $10
exit r1", true)]
		[TestCase(@"mov r1 $10 ; Move 10 into r1
exit r1 ; Exit with r1
; Done!
", true)]
		[TestCase(TestPrograms.ReallySimpleProgram, true)]
		[TestCase(TestPrograms.P1, true)]
		[TestCase(TestPrograms.P2, true)]
		public void ParseProgram(string value, bool expected)
		{
			var grammer = new AsmStructuralGrammar();
			var p = new ParserState(value);

			bool parseResult = grammer.program.Match(p);
			var printer = new CppStructuralOutputAsXml();
			printer.Print(p.GetRoot());
			Console.WriteLine(printer.AsXml());

			Assert.That(parseResult, Is.EqualTo(expected));
		}
	}
}