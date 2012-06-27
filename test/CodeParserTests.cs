using System;
using System.IO;
using Andy.TinyOS.Utility;
using NUnit.Framework;
using tinyOS;

namespace ClassLibrary1
{
	[TestFixture]
	public class CodeParserTests
	{
		[TestCase(@"D:\Users\andy\Documents\GitHub\TinyOS\Sample Programs\prog1.txt")]
		[TestCase(@"D:\Users\andy\Documents\GitHub\TinyOS\Sample Programs\prog2.txt")]
		[TestCase(@"D:\Users\andy\Documents\GitHub\TinyOS\Sample Programs\prog3.txt")]
		[TestCase(@"D:\Users\andy\Documents\GitHub\TinyOS\Sample Programs\scott1.txt")]
		[TestCase(@"D:\Users\andy\Documents\GitHub\TinyOS\Sample Programs\scott2.txt")]
		[TestCase(@"D:\Users\andy\Documents\GitHub\TinyOS\Sample Programs\scott3.txt")]
		[TestCase(@"D:\Users\andy\Documents\GitHub\TinyOS\Sample Programs\scott4.txt")]
		[TestCase(@"D:\Users\andy\Documents\GitHub\TinyOS\Sample Programs\scott5.txt")]
		[TestCase(@"D:\Users\andy\Documents\GitHub\TinyOS\Sample Programs\scott6.txt")]
		[TestCase(@"D:\Users\andy\Documents\GitHub\TinyOS\Sample Programs\scott7.txt")]
		[TestCase(@"D:\Users\andy\Documents\GitHub\TinyOS\Sample Programs\scott8.txt")]
		[TestCase(@"D:\Users\andy\Documents\GitHub\TinyOS\Sample Programs\scott9.txt")]
		[TestCase(@"D:\Users\andy\Documents\GitHub\TinyOS\Sample Programs\scott10.txt")]
		[TestCase(@"D:\Users\andy\Documents\GitHub\TinyOS\Sample Programs\scott11.txt")]
		[TestCase(@"D:\Users\andy\Documents\GitHub\TinyOS\Sample Programs\scott12.txt")]
		[TestCase(@"D:\Users\andy\Documents\GitHub\TinyOS\Sample Programs\scott13.txt")]
		public void OpenFile(string file)
		{
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

		[TestCase(@"D:\Users\andy\Documents\GitHub\TinyOS\Sample Programs\prog1.txt")]
		[TestCase(@"D:\Users\andy\Documents\GitHub\TinyOS\Sample Programs\prog2.txt")]
		[TestCase(@"D:\Users\andy\Documents\GitHub\TinyOS\Sample Programs\prog3.txt")]
		[TestCase(@"D:\Users\andy\Documents\GitHub\TinyOS\Sample Programs\scott1.txt")]
		[TestCase(@"D:\Users\andy\Documents\GitHub\TinyOS\Sample Programs\scott2.txt")]
		[TestCase(@"D:\Users\andy\Documents\GitHub\TinyOS\Sample Programs\scott3.txt")]
		[TestCase(@"D:\Users\andy\Documents\GitHub\TinyOS\Sample Programs\scott4.txt")]
		[TestCase(@"D:\Users\andy\Documents\GitHub\TinyOS\Sample Programs\scott5.txt")]
		[TestCase(@"D:\Users\andy\Documents\GitHub\TinyOS\Sample Programs\scott6.txt")]
		[TestCase(@"D:\Users\andy\Documents\GitHub\TinyOS\Sample Programs\scott7.txt")]
		[TestCase(@"D:\Users\andy\Documents\GitHub\TinyOS\Sample Programs\scott8.txt")]
		[TestCase(@"D:\Users\andy\Documents\GitHub\TinyOS\Sample Programs\scott9.txt")]
		[TestCase(@"D:\Users\andy\Documents\GitHub\TinyOS\Sample Programs\scott10.txt")]
		[TestCase(@"D:\Users\andy\Documents\GitHub\TinyOS\Sample Programs\scott11.txt")]
		[TestCase(@"D:\Users\andy\Documents\GitHub\TinyOS\Sample Programs\scott12.txt")]
		[TestCase(@"D:\Users\andy\Documents\GitHub\TinyOS\Sample Programs\scott13.txt")]
		public void Decompile(string filename)
		{
			var objData = Compile(@filename);
			var reader = new CodeReader(objData);
			var writer = new InstructionTextWriter(Console.Out);
			foreach  (var instruction in reader.Instructions)
			{
				writer.Write(instruction);
			}
		}

		private byte[] Compile(string file)
		{
			var parser = new InstructionTextReader();
			var ms = new MemoryStream();
			var writer = new tinyOS.CodeWriter(ms);

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
			var badInstruction = new Instruction { OpCode = OpCode.Ret, Parameters = new[] { 0U } };
			Assert.That(badInstruction.ToString(), Is.StringMatching(@"^Ret\s+Unknown \(0\)"));

			var ms = new StringWriter();
			using (var writer = new InstructionTextWriter(ms))
				writer.Write(badInstruction);
		}

		[Test, ExpectedException(typeof(InvalidOperationException))]
		public void WriteBadOpCode()
		{
			var badInstruction = new Instruction {OpCode = (OpCode) 254 };
			Assert.That(badInstruction.ToString(), Is.StringMatching(@"^254$"));
			
			var ms = new StringWriter();
			using (var writer = new InstructionTextWriter(ms))
				writer.Write(badInstruction);
		}

		[Test, ExpectedException(typeof(InvalidOperationException))]
		public void WriteMissingParameter()
		{
			var badInstruction = new Instruction { OpCode = OpCode.Movi, Parameters = new[] { 0U } };
			Assert.That(badInstruction.ToString(), Is.StringMatching(@"^Movi\s+r1"));

			var ms = new StringWriter();
			using (var writer = new InstructionTextWriter(ms))
			{
				writer.Write(badInstruction);
			}
		}
	}
}