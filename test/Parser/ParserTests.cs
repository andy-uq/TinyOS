using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using Andy.TinyOS.Compiler;
using Andy.TinyOS.Parser;
using Jolt.Testing.Assertions;
using Jolt.Testing.Assertions.NUnit.SyntaxHelpers;
using Moq;
using NUnit.Framework;

namespace ClassLibrary1.Parser
{
	[TestFixture]
	public class ParserTests
	{
		private readonly Dictionary<string, string> _testPaths = new Dictionary<string, string>()
		{
			{"TBPC16", @"D:\Users\andy\Documents\GitHub\TinyOS\"},
			{"ARCHANGEL", @"C:\Users\andy\Documents\GitHub\TinyOS\"}
		};

		private readonly string Source_Directory = @"test\Parser\Input";
		private readonly string Output_Directory = @"test\Parser\Output";

		public ParserTests()
		{
			Source_Directory = Path.Combine(_testPaths[Environment.MachineName], Source_Directory);
			Output_Directory = Path.Combine(_testPaths[Environment.MachineName], Output_Directory);
		}

		[TestCase("test01.c")]
		[TestCase("test02.c")]
		[TestCase("test03.c")]
		[TestCase("test04.c")]
		[TestCase("larger.c")]
		public void Can_parse(string source)
		{
			var sourceFile = Path.Combine(Source_Directory, source);
			if (!File.Exists(sourceFile))
				Assert.Inconclusive("Cannot find filename " + sourceFile);

			var output = new CppStructuralOutputAsXml();
            
			var result = CppFileParser.Parse(sourceFile, output);
			Console.WriteLine(result.GetRoot());
			Assert.That(result.ParseException, Is.Null);

			Console.WriteLine("Actual");
			Console.WriteLine(output.AsXml());
			Console.WriteLine();

			var expectedXml = XDocument.Load(Path.Combine(Output_Directory, Path.ChangeExtension(source, ".xml")));
			Console.WriteLine("Expected");
			Console.WriteLine(expectedXml);

			Assert.That(output.AsXml().CreateReader(), IsXml.EquivalentTo(expectedXml.CreateReader()));
		}

		[TestCase("0", true)]
		[TestCase("1", true)]
		[TestCase("10", true)]
		[TestCase("100000000", true)]
		[TestCase("-1", false)]
		[TestCase("abc", false)]
		public void Integer(string integer, bool expected)
		{
			var grammer = new AndyStructuralGrammar();
			var p = new ParserState(integer);
			
			Assert.That(grammer.int_literal.Match(p), Is.EqualTo(expected));
		}

		[TestCase("0", true)]
		[TestCase("1", true)]
		[TestCase("10", true)]
		[TestCase("-1", true)]
		[TestCase("-10", true)]
		[TestCase("-100000000", true)]
		[TestCase("!1", true)]
		[TestCase("-(1)", true)]
		public void UnaryInteger(string integer, bool expected)
		{
			var grammer = new AndyStructuralGrammar();
			var p = new ParserState(integer);

			Assert.That(grammer.term.Match(p), Is.EqualTo(expected));
			
			var printer = new CppStructuralOutputAsXml();
			printer.Print(p.GetRoot());
			Console.WriteLine(printer.AsXml());
		}

		[TestCase("1+1", true)]
		[TestCase("1*2+3", true)]
		[TestCase("1*2+3/2", true)]
		[TestCase("3&2", true)]
		[TestCase("3&2&1", true)]
		[TestCase("a+1", true)]
		[TestCase("a=10", true)]
		[TestCase("a = 10", true)]
		public void Expression(string integer, bool expected)
		{
			var grammer = new AndyStructuralGrammar();
			var printer = new CppStructuralOutputAsXml();
			var p = new ParserState(integer);

			Assert.That(grammer.expression.Match(p), Is.EqualTo(expected));

			printer.Print(p.GetRoot());
			Console.WriteLine(printer.AsXml());
		}

		[TestCase("a", true)]
		public void Variable(string expression, bool expected)
		{
			var grammer = new AndyStructuralGrammar();
			var printer = new CppStructuralOutputAsXml();
			var p = new ParserState(expression);

			Assert.That(grammer.identifier.Match(p), Is.EqualTo(expected));

			printer.Print(p.GetRoot());
			Console.WriteLine(printer.AsXml());
		}

		[TestCase("a()", true)]
		public void Function(string expression, bool expected)
		{
			var grammer = new AndyStructuralGrammar();
			var printer = new CppStructuralOutputAsXml();
			var p = new ParserState(expression);

			Assert.That(grammer.function_call.Match(p), Is.EqualTo(expected));

			printer.Print(p.GetRoot());
			Console.WriteLine(printer.AsXml());
		}

		[Test]
		public void BuildWhileStatement()
		{
			var grammer = new AndyStructuralGrammar();
			var p = new ParserState("while");

			Assert.That(grammer.while_keyword.Match(p), Is.True);

			p = new ParserState("while()");
			var rule = grammer.while_keyword + grammer.Delimiter("(") + grammer.Delimiter(")");
			Assert.That(rule.Match(p), Is.True);

			p = new ParserState("while (a == b)");
			Assert.That(grammer.while_condition.Match(p), Is.True);
			
			p = new ParserState("while(a == 10){a=20;}");
			var while_block = grammer.while_condition + grammer.block;
			Assert.That(while_block.Match(p), Is.True);
			
			p = new ParserState("while(a == 10){a=20;}");
			Assert.That(grammer.while_statement.Match(p), Is.True);

			p = new ParserState("while(a == 10){a=20;}");
			Assert.That(grammer.control_statement.Match(p), Is.True);
		}

		[Test]
		public void BuildIfStatement()
		{
			var grammer = new AndyStructuralGrammar();
			var p = new ParserState("if");

			Assert.That(grammer.if_keyword.Match(p), Is.True);
			
			p = new ParserState("if()");
			var rule = grammer.if_keyword + grammer.Delimiter("(") + grammer.Delimiter(")");
			Assert.That(rule.Match(p), Is.True);

			p = new ParserState("if (a == b)");
			Assert.That(grammer.if_condition.Match(p), Is.True);

			p = new ParserState("if(a == 10){a=20;}");
			var if_block = grammer.if_condition + grammer.block;
			Assert.That(if_block.Match(p), Is.True);

			p = new ParserState("if(a == 10){a=20;}");
			if_block = grammer.if_condition + new RecursiveRule(() => grammer.block);
			Assert.That(if_block.Match(p), Is.True);

			p = new ParserState("if(a == 10){a=20;}");
			Assert.That(grammer.control_statement.Match(p), Is.True);
		}

		[Test]
		public void BuildElseStatement()
		{
			var grammer = new AndyStructuralGrammar();
			var p = new ParserState("else");

			Assert.That(grammer.else_keyword.Match(p), Is.True);

			p = new ParserState("else { b=10; }");
			Assert.That(grammer.else_block.Match(p), Is.True);

			p = new ParserState("if(a == 10){a=20;}");
			var if_block = grammer.if_condition + grammer.block + new OptRule(grammer.else_block);
			Assert.That(if_block.Match(p), Is.True);

			p = new ParserState("if(a == 10){a=20;}else{b=10;}");
			Assert.That(if_block.Match(p), Is.True);

			p = new ParserState("if(a == 10){ a=20; } else { b=10; }");
			Assert.That(grammer.if_statement.Match(p), Is.True);
		}

		[TestCase("a = 10;", true)]
		[TestCase("if ( a == 10 ) { a=20; }", true)]
		[TestCase("if ( a == 10 ) { a=20; } else { a=10; }", true)]
		public void Statement(string expression, bool expected)
		{
			var grammer = new AndyStructuralGrammar();
			var p = new ParserState(expression);

			Assert.That(grammer.statement.Match(p), Is.EqualTo(expected));

			var printer = new CppStructuralOutputAsXml();
			printer.Print(p.GetRoot());
			Console.WriteLine(printer.AsXml());
		}

		[TestCase("{ a = 10; }", true)]
		[TestCase("{ a = 10; b = 20; c = a + b; }", true)]
		public void Block(string expression, bool expected)
		{
			var grammer = new AndyStructuralGrammar();
			var p = new ParserState(expression);

			Assert.That(grammer.block.Match(p), Is.EqualTo(expected));

			var printer = new CppStructuralOutputAsXml();
			printer.Print(p.GetRoot());
			Console.WriteLine(printer.AsXml());
		}

		[Test]
		public void ParseComment()
		{
			var grammar = new CppStructuralGrammar();
			var output = new CppStructuralOutputAsXml();
			var parserState = CppSourceParser.Parse("/* Comment */", output, rule: grammar.declaration_list);
			Assert.That(parserState.ParseException, Is.Null);

			Console.WriteLine(output.AsXml());
		}

		[Test]
		public void ParseAssignment()
		{
			var parserState = CppSourceParser.Parse("/* Comment */ int main() { int i = 1; }");
			Assert.That(parserState.ParseException, Is.Null);

			var root = parserState.GetRoot();
			Console.WriteLine(root);
		}
	}
}