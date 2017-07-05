using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Andy.TinyOS;
using Andy.TinyOS.Compiler;
using Andy.TinyOS.Parser;
using Andy.TinyOS.Utility;
using FluentAssertions;
using Xunit;

namespace ClassLibrary1
{
	public class CodeParserTests
	{
		private readonly Dictionary<string, string> _testPaths = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
		{
			{"TBPC16", @"D:\Users\andy\Documents\GitHub\TinyOS\"},
			{"ARCHANGEL", @"C:\Users\andy\Documents\GitHub\TinyOS\"},
			{"ANDYCLARKE",@"C:\Dotnet\TinyOS\"},
			{"ANDYLAPTOP",@"C:\Source\C#\TinyOS\"},
		};

		[Theory]
		[InlineData(@"Sample Programs\prog1.txt")]
		[InlineData(@"Sample Programs\prog2.txt")]
		[InlineData(@"Sample Programs\prog3.txt")]
		[InlineData(@"Sample Programs\scott1.txt")]
		[InlineData(@"Sample Programs\scott2.txt")]
		[InlineData(@"Sample Programs\scott3.txt")]
		[InlineData(@"Sample Programs\scott4.txt")]
		[InlineData(@"Sample Programs\scott5.txt")]
		[InlineData(@"Sample Programs\scott6.txt")]
		[InlineData(@"Sample Programs\scott7.txt")]
		[InlineData(@"Sample Programs\scott8.txt")]
		[InlineData(@"Sample Programs\scott9.txt")]
		[InlineData(@"Sample Programs\scott10.txt")]
		[InlineData(@"Sample Programs\scott11.txt")]
		[InlineData(@"Sample Programs\scott12.txt")]
		[InlineData(@"Sample Programs\scott13.txt")]
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
				result.Should().BeTrue();
				
				var printer = new CppStructuralOutputAsXml();
				printer.Print(parser.GetRoot());
				Console.WriteLine(printer.AsXml());

				var assembler = new Assembler(parser);
				assembler.Assemble().ToList().ForEach(writer.Write);
			}
		}

		[Theory]
		[InlineData(@"Sample Programs\prog1.txt")]
		[InlineData(@"Sample Programs\prog2.txt")]
		[InlineData(@"Sample Programs\prog3.txt")]
		[InlineData(@"Sample Programs\scott1.txt")]
		[InlineData(@"Sample Programs\scott2.txt")]
		[InlineData(@"Sample Programs\scott3.txt")]
		[InlineData(@"Sample Programs\scott4.txt")]
		[InlineData(@"Sample Programs\scott5.txt")]
		[InlineData(@"Sample Programs\scott6.txt")]
		[InlineData(@"Sample Programs\scott7.txt")]
		[InlineData(@"Sample Programs\scott8.txt")]
		[InlineData(@"Sample Programs\scott9.txt")]
		[InlineData(@"Sample Programs\scott10.txt")]
		[InlineData(@"Sample Programs\scott11.txt")]
		[InlineData(@"Sample Programs\scott12.txt")]
		[InlineData(@"Sample Programs\scott13.txt")]
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

		[Fact]
		public void ParseExpression()
		{
			var instruction = new Instruction(OpCode.Mov).Destination(Register.A).Source(10);
			instruction.OpCode.Should().Be(OpCode.Mov);
			instruction.OpCodeMask.Should().Be(13);
			Xunit.Assert.Matches(@"^Mov\s+r1\s+\$10", instruction.ToString());
		}

		[Fact]
		public void WriteExtraParameter()
		{
			var badInstruction = new Instruction(OpCode.Ret).Source(10);
			Xunit.Assert.Matches(@"^Ret\s+\$10", badInstruction.ToString());

			var ms = new StringWriter();
			using (var writer = new InstructionTextWriter(ms))
			{
				Xunit.Assert.Throws<InvalidOperationException>(() => writer.Write(badInstruction));
			}
		}

		[Fact]
		public void WriteBadOpCode()
		{
			var badInstruction = new Instruction((OpCode) 254);
			Xunit.Assert.Matches(@"^254\s+;.*", badInstruction.ToString());
			
			var ms = new StringWriter();
			using (var writer = new InstructionTextWriter(ms))
			{
				Xunit.Assert.Throws<InvalidOperationException>(() => writer.Write(badInstruction));
			}
		}

		[Fact]
		public void WriteMissingParameter()
		{
			var badInstruction = new Instruction(OpCode.Mov).Source(10);
			Xunit.Assert.Matches("^Mov", badInstruction.ToString());

			var ms = new StringWriter();
			using (var writer = new InstructionTextWriter(ms))
			{
				Xunit.Assert.Throws<InvalidOperationException>(() => writer.Write(badInstruction));
			}
		}
	}
}