using System;
using System.Collections.Generic;
using System.IO;
using Andy.TinyOS;
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
			{"TBPC16", @"D:\Users\andy\Documents\GitHub\TinyOS\"}
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
			file = string.Concat(_testPaths[Environment.MachineName], file);
			
			var parser = new InstructionTextReader();
			var outputFile = Path.ChangeExtension(file, ".asm");
			using (var stringWriter = File.CreateText(outputFile))
			using (var writer = new InstructionTextWriter(stringWriter))
			using (var streamReader = File.OpenText(file))
			{
				string line;
				while ((line = streamReader.ReadLine()) != null)
				{
					var i = parser.Parse(line);
					writer.Write(i);
				}
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
			var parser = new InstructionTextReader();
			var ms = new MemoryStream();
			var writer = new CodeWriter(ms);

			using (var streamReader = File.OpenText(file))
			{
				string line;
				while ((line = streamReader.ReadLine()) != null)
				{
					var i = parser.Parse(line);
					writer.Write(i);
				}
			}

			writer.Close();
			return ms.ToArray();
		}

		[Test, ExpectedException(typeof(InvalidOperationException))]
		public void WriteExtraParameter()
		{
			var badInstruction = new Instruction(OpCode.Ret).Source(10);
			Assert.That(badInstruction.ToString(), Is.StringMatching(@"^Ret\s+Unknown \(10\)"));

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
			var badInstruction = new Instruction(OpCode.Movi) { Parameters = new[] { 0U } };
			Assert.That(badInstruction.ToString(), Is.StringMatching(@"^Movi\s+r1"));

			var ms = new StringWriter();
			using (var writer = new InstructionTextWriter(ms))
			{
				writer.Write(badInstruction);
			}
		}
	}
}