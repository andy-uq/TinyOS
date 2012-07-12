using System;
using System.IO;
using System.Xml.Linq;
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
		private const string Source_Directory = @"D:\Users\andy\Documents\GitHub\TinyOS\test\Parser\Input";
		private const string Output_Directory = @"D:\Users\andy\Documents\GitHub\TinyOS\test\Parser\Output";

		[TestCase("test01.c")]
		[TestCase("test02.c")]
		[TestCase("test03.c")]
		public void Can_parse(string source)
		{
			var sourceFile = Path.Combine(Source_Directory, source);
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