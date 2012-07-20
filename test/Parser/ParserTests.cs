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
			{"TBPC16", @"D:\Users\andy\Documents\GitHub\TinyOS\"}
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
			var printer = new CppStructuralOutputAsXml();
			var p = new ParserState(integer);

			Assert.That(grammer.term.Match(p), Is.EqualTo(expected));
			
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