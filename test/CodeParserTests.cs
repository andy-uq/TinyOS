using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Andy.TinyOS;
using Andy.TinyOS.Compiler;
using Andy.TinyOS.Parser;
using Andy.TinyOS.Utility;
using NUnit.Framework;
using tinyOS;

namespace ClassLibrary1
{
	[TestFixture]
	public class CodeParserTests
	{
		private readonly Dictionary<string, string> _testPaths = new Dictionary<string, string>()
		{
			{"TBPC16", @"D:\Users\andy\Documents\GitHub\TinyOS\"},
			{"ARCHANGEL", @"C:\Users\andy\Documents\GitHub\TinyOS\"}
		};

		[TestCase(@"Sample Programs\prog1.txt")]
		[TestCase(@"Sample Programs\prog2.txt")]
		[TestCase(@"Sample Programs\prog3.txt")]
		[TestCase(@"Sample Programs\scott1.txt")]
		[TestCase(@"Sample Programs\scott2.txt")]
		[TestCase(@"Sample Programs\scott3.txt")]
		[TestCase(@"Sample Programs\scott4.txt")]
		[TestCase(@"Sample Programs\scott5.txt")]
		[TestCase(@"Sample Programs\scott6.txt")]
		[TestCase(@"Sample Programs\scott7.txt")]
		[TestCase(@"Sample Programs\scott8.txt")]
		[TestCase(@"Sample Programs\scott9.txt")]
		[TestCase(@"Sample Programs\scott10.txt")]
		[TestCase(@"Sample Programs\scott11.txt")]
		[TestCase(@"Sample Programs\scott12.txt")]
		[TestCase(@"Sample Programs\scott13.txt")]
		public void OpenFile(string file)
		{
			var grammar = new AsmStructuralGrammar();
			file = string.Concat(_testPaths[Environment.MachineName], file);
			
			var outputFile = Path.ChangeExtension(file, ".asm");
			using (var stringWriter = File.CreateText(outputFile))
			using (var writer = new InstructionTextWriter(stringWriter))
			{
				string source = File.ReadAllText(file);
				var parser = new ParserState(source);
				var result = grammar.program.Match(parser);
				Assert.That(result, Is.True);

				var printer = new CppStructuralOutputAsXml();
				printer.Print(parser.GetRoot());
				Console.WriteLine(printer.AsXml());

				var assembler = new Assembler(parser);
				assembler.Assemble().ToList().ForEach(writer.Write);
			}
		}

		[TestCase(@"Sample Programs\prog1.txt")]
		[TestCase(@"Sample Programs\prog2.txt")]
		[TestCase(@"Sample Programs\prog3.txt")]
		[TestCase(@"Sample Programs\scott1.txt")]
		[TestCase(@"Sample Programs\scott2.txt")]
		[TestCase(@"Sample Programs\scott3.txt")]
		[TestCase(@"Sample Programs\scott4.txt")]
		[TestCase(@"Sample Programs\scott5.txt")]
		[TestCase(@"Sample Programs\scott6.txt")]
		[TestCase(@"Sample Programs\scott7.txt")]
		[TestCase(@"Sample Programs\scott8.txt")]
		[TestCase(@"Sample Programs\scott9.txt")]
		[TestCase(@"Sample Programs\scott10.txt")]
		[TestCase(@"Sample Programs\scott11.txt")]
		[TestCase(@"Sample Programs\scott12.txt")]
		[TestCase(@"Sample Programs\scott13.txt")]
		public void Decompile(string file)
		{
			file = string.Concat(_testPaths[Environment.MachineName], file);

			var objData = Compile(file);
			var reader = new CodeReader(objData);
			var writer = new InstructionTextWriter(Console.Out);
			foreach  (var instruction in reader)
			{
				writer.Write(instruction);
			}
		}

		private byte[] Compile(string file)
		{
			var ms = new MemoryStream();
			using (var writer = new CodeWriter(ms))
			{
				string source = File.ReadAllText(file);
				var assembler = new Assembler(source);
				assembler.Assemble().ToList().ForEach(writer.Write);
			}

			return ms.ToArray();
		}

		[Test]
		public void ParseExpression()
		{
			var instruction = new Instruction(OpCode.Mov).Destination(Register.A).Source(10);
			Assert.That(instruction.OpCode, Is.EqualTo(OpCode.Mov));
			Assert.That(instruction.OpCodeMask, Is.EqualTo(13));
			Assert.That(instruction.ToString(), Is.StringMatching(@"^Mov\s+r1\s+\$10"));
		}

		[Test, ExpectedException(typeof(InvalidOperationException))]
		public void WriteExtraParameter()
		{
			var badInstruction = new Instruction(OpCode.Ret).Source(10);
			Assert.That(badInstruction.ToString(), Is.StringMatching(@"^Ret\s+\$10"));

			var ms = new StringWriter();
			using (var writer = new InstructionTextWriter(ms))
				writer.Write(badInstruction);
		}

		[Test, ExpectedException(typeof(InvalidOperationException))]
		public void WriteBadOpCode()
		{
			var badInstruction = new Instruction((OpCode) 254);
			Assert.That(badInstruction.ToString(), Is.StringMatching(@"^254\s+;.*"));
			
			var ms = new StringWriter();
			using (var writer = new InstructionTextWriter(ms))
				writer.Write(badInstruction);
		}

		[Test, ExpectedException(typeof(InvalidOperationException))]
		public void WriteMissingParameter()
		{
			var badInstruction = new Instruction(OpCode.Mov).Source(10);
			Assert.That(badInstruction.ToString(), Is.StringMatching(@"^Mov"));

			var ms = new StringWriter();
			using (var writer = new InstructionTextWriter(ms))
			{
				writer.Write(badInstruction);
			}
		}
	}
}