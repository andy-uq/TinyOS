using System;
using System.IO;
using NUnit.Framework;
using tinyOS;

namespace ClassLibrary1
{
	[TestFixture]
	public class CodeParserTests
	{
		[Test]
		public void OpenFile()
		{
			var file = @"C:\Users\andy\Documents\GitHub\TinyOS\Sample Programs\scott13.txt";
			var parser = new TextParser();
			var ms = new StringWriter();
			var writer = new tinyOS.InstructionFormatter(ms);

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
			Console.WriteLine(ms);
		}

		[Test]
		public void Compile()
		{
			var objData = Compile(@"C:\Users\andy\Documents\GitHub\TinyOS\Sample Programs\scott13.txt");
			Console.WriteLine(BitConverter.ToString(objData));
		}

		[Test]
		public void Decompile()
		{
			var objData = Compile(@"C:\Users\andy\Documents\GitHub\TinyOS\Sample Programs\scott13.txt");
			var reader = new CodeReader(objData);
			var writer = new InstructionFormatter(Console.Out);
			foreach  (var instruction in reader.Instructions)
			{
				writer.Write(instruction);
			}
		}

		private static byte[] Compile(string file)
		{
			var parser = new TextParser();
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
		public void WriteBadCode()
		{
			var ms = new StringWriter();
			var writer = new tinyOS.InstructionFormatter(ms);
			writer.Write(new Instruction { OpCode = OpCode.Ret, Parameters = new[] { 0U }});
		}
	}
}